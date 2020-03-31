using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using McMaster.Extensions.CommandLineUtils;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Diagnostics.Commands;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Terminal.Gui;
using TerminalAttribute = Terminal.Gui.Attribute;

[assembly: Convention(typeof(ConventionCommandConvention))]

namespace Rocket.Surgery.Conventions.Diagnostics.Commands
{
    [Command(
        "list",
        Description = "Applies all outstanding changes to the database based on the current configuration"
    )]
    internal class ConventionListCommand
    {
        private static (FrameView frame, ListView list) BuildListFrameView(
            string name,
            View? relativeTo = null,
            bool canFocus = false
        )
        {
            var frame = new FrameView(name)
            {
                X = relativeTo != null ? Pos.Right(relativeTo) : null,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var list = new ListView(Array.Empty<string>())
            {
                AllowsMarking = false,
                CanFocus = canFocus
            };
            frame.Add(list);

            return ( frame, list );
        }

        private static (Label label, Label value) BuildLabelView(string name, View? relativeTo = null, int width = 12)
        {
            var label = new Label(name)
            {
                X = 1,
                Y = relativeTo == null ? 0 : Pos.Bottom(relativeTo),
                Height = 1,
                Width = width
            };
            var value = new Label("")
            {
                X = Pos.Right(label),
                Y = relativeTo == null ? 0 : Pos.Bottom(relativeTo),
                Width = Dim.Fill(),
                Height = 1,
                CanFocus = false
            };

            return ( label, value );
        }

        private readonly IConventionScanner _scanner;
        private readonly IAssemblyCandidateFinder _assemblyCandidateFinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionListCommand" /> class.
        /// </summary>
        public ConventionListCommand(
            IConventionScanner scanner,
            IAssemblyCandidateFinder assemblyCandidateFinder
        )
        {
            _scanner = scanner;
            _assemblyCandidateFinder = assemblyCandidateFinder;
        }

        /// <summary>
        /// Called when [execute].
        /// </summary>
        /// <returns>System.Int32.</returns>
        [UsedImplicitly]
        public int OnExecute()
        {
            Application.Init();

            var top = new CustomWindow
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            top.ColorScheme = new ColorScheme
            {
                Focus = TerminalAttribute.Make(Color.Brown, Color.Black),
                HotFocus = TerminalAttribute.Make(Color.BrightYellow, Color.Black),
                HotNormal = TerminalAttribute.Make(Color.Gray, Color.Black),
                Normal = TerminalAttribute.Make(Color.White, Color.Black)
            };

            var discovery = CreateDiscoveryWindow();
            discovery.ColorScheme = top.ColorScheme;

            var locate = CreateLocateWindow();
            locate.ColorScheme = top.ColorScheme;

            var menu = new MenuBar(
                new[]
                {
                    new MenuBarItem(
                        "_File",
                        new[]
                        {
                            new MenuItem(
                                "_Discovery",
                                "Discover registered conventions and their orders",
                                openDiscovery
                            ),
                            new MenuItem(
                                "_Locate",
                                "Locate all available conventions from different assemblies",
                                openLocate
                            )
                        }
                    )
                }
            );

            top.Add(menu);

            void openDiscovery()
            {
                top.Remove(locate);
                top.Add(discovery);
                Application.RequestStop();
                Application.Run(top);
            }

            void openLocate()
            {
                top.Remove(discovery);
                top.Add(locate);
                Application.RequestStop();
                Application.Run(top);
            }

            openDiscovery();

            return 0;
        }

        private DiscoveryWindow CreateDiscoveryWindow()
        {
            var builders = _assemblyCandidateFinder.GetCandidateAssemblies(
                    "Rocket.Surgery.Conventions.Abstractions",
                    "Rocket.Surgery.Conventions.Attributes",
                    "Rocket.Surgery.Conventions"
                )
               .SelectMany(x => x.DefinedTypes)
               .Where(
                    x => x.IsInterface && x.ImplementedInterfaces.Any(
                        z => z.IsGenericType && typeof(IConventionContainer<,,>) == z.GetGenericTypeDefinition()
                    )
                )
               .Distinct()
               .Select((x, i) => new ConventionDefinition(x))
               .OrderBy(x => x.Name)
               .ToArray();
            return new DiscoveryWindow(_scanner).UpdateDefinitions(builders);
        }

        private LocateWindow CreateLocateWindow()
        {
            var builders = _assemblyCandidateFinder.GetCandidateAssemblies(
                    "Rocket.Surgery.Conventions.Abstractions",
                    "Rocket.Surgery.Conventions.Attributes",
                    "Rocket.Surgery.Conventions"
                )
               .SelectMany(x => x.DefinedTypes)
               .Where(
                    x => x.IsInterface && x.ImplementedInterfaces.Any(
                        z => z.IsGenericType && typeof(IConventionContainer<,,>) == z.GetGenericTypeDefinition()
                    )
                )
               .Distinct()
               .Select((x, i) => new ConventionDefinition(x))
               .OrderBy(x => x.Name)
               .ToArray();
            var candidateAssemblies = _assemblyCandidateFinder.GetCandidateAssemblies(
                    "Rocket.Surgery.Conventions.Abstractions",
                    "Rocket.Surgery.Conventions.Attributes",
                    "Rocket.Surgery.Conventions"
                )
               .Select(x => new AssemblyDefinition(x))
               .Where(x => x.Conventions.Any())
               .ToArray();
            return new LocateWindow().UpdateAssemblies(candidateAssemblies).UpdateDefinitions(builders);
        }

        private class DiscoveryWindow : Window
        {
            private readonly (FrameView frame, ListView list) _conventions;
            private readonly (FrameView frame, ListView list) _discovered;
            private readonly FrameView _details;
            private readonly (Label label, Label value) _kindView;
            private readonly (Label label, Label value) _assemblyView;
            private readonly (Label label, Label value) _typeView;
            private readonly IConventionScanner _scanner;
            private ConventionDefinition[] _definitions = Array.Empty<ConventionDefinition>();
            private ConventionDetails[] _conventionDetails = Array.Empty<ConventionDetails>();

            public DiscoveryWindow(IConventionScanner scanner) : base("Discovery")
            {
                Y = 1;
                _conventions = BuildListFrameView("Conventions", canFocus: true);
                Add(_conventions.frame);

                _discovered = BuildListFrameView("Discovered", _conventions.frame, true);
                Add(_discovered.frame);

                _details = new FrameView("Details")
                {
                    X = Pos.Right(_discovered.frame),
                    Width = Dim.Fill(),
                    Height = Dim.Fill()
                };
                Add(_details);

                _kindView = BuildLabelView("Kind:");
                _details.Add(_kindView.label, _kindView.value);
                _assemblyView = BuildLabelView("Assembly:", _kindView.label);
                _details.Add(_assemblyView.label, _assemblyView.value);
                _typeView = BuildLabelView("Type:", _assemblyView.label);
                _details.Add(_typeView.label, _typeView.value);

                _conventions.list.SelectedChanged += UpdateConventions;
                _discovered.list.SelectedChanged += UpdateDetails;
                _scanner = scanner;
            }

            public sealed override void Add(View view) => base.Add(view);

            public DiscoveryWindow UpdateDefinitions(ConventionDefinition[] definitions)
            {
                _definitions = definitions;
                _conventions.frame.Width = definitions.Max(x => x.Name.Length) + 4;
                _conventions.list.SetSource(definitions);
                UpdateConventions();
                return this;
            }

            private void UpdateConventions()
            {
                if (_details.Frame.Width == 0)
                {
                    Application.Current.SetNeedsDisplay();
                    Application.Current.Redraw(Application.Current.Bounds);
                }

                if (_conventions.list.Source.Count == 0)
                {
                    UpdateDetails();
                    return;
                }

                var builder = _definitions[_conventions.list.SelectedItem];
                _conventionDetails = _scanner.BuildProvider().GetAll()
                   .Where(
                        x => ( x.Convention != null &&
                            builder.ConventionType.IsAssignableFrom(x.Convention.GetType()) ) || ( x.Delegate != null &&
                            builder.DelegateType.IsAssignableFrom(x.Delegate?.GetType()) )
                    )
                   .Select((cod, index) => new ConventionDetails(cod, index))
                   .ToArray();
                _discovered.list.SetSource(_conventionDetails);
                UpdateDetails();
            }

            private void UpdateDetails()
            {
                if (_discovered.list.Source.Count == 0)
                {
                    _assemblyView.value.Text = "";
                    _kindView.value.Text = "";
                    _typeView.value.Text = "";
                    return;
                }

                _discovered.frame.Width = _conventionDetails.Max(x => x.ToString().Length) + 4;
                var detail = _conventionDetails[_discovered.list.SelectedItem];
                _assemblyView.value.Text = MinLabelText(detail.Assembly.GetName().FullName);
                _kindView.value.Text = MinLabelText(detail.Kind.ToString());
                _typeView.value.Text = MinLabelText(detail.Type.FullName);
            }

            private string MinLabelText(string? value) => ( value ?? string.Empty ).Substring(
                0,
                _details.Frame.Width <= 0
                    ? ( value ?? string.Empty ).Length
                    : Math.Min(
                        ( value ?? string.Empty ).Length,
                        _details.Frame.Width - _assemblyView.label.Frame.Width - 4
                    )
            );
        }

        private class LocateWindow : Window
        {
            private readonly (FrameView frame, ListView list) _assembly;
            private readonly (FrameView frame, ListView list) _conventions;
            private readonly FrameView _details;
            private readonly (Label label, Label value) _isPublicView;
            private readonly (Label label, Label value) _hasAttributeView;
            private readonly (Label label, Label value) _assemblyView;
            private readonly (Label label, Label value) _typeView;
            private readonly Label _conventionsValue;
            private AssemblyDefinition[] _assemblies = Array.Empty<AssemblyDefinition>();
            private ConventionDefinition[] _definitions = Array.Empty<ConventionDefinition>();
            private AssemblyConventionDetails[] _conventionDetails = Array.Empty<AssemblyConventionDetails>();

            public LocateWindow() : base("Discovery")
            {
                Y = 1;
                _assembly = BuildListFrameView("Assemblies", canFocus: true);
                Add(_assembly.frame);

                _conventions = BuildListFrameView("Conventions", _assembly.frame, true);
                Add(_conventions.frame);

                _details = new FrameView("Details")
                {
                    X = Pos.Right(_conventions.frame),
                    Width = Dim.Fill(),
                    Height = Dim.Fill()
                };
                Add(_details);

                _assemblyView = BuildLabelView("Assembly:");
                _details.Add(_assemblyView.label, _assemblyView.value);

                _typeView = BuildLabelView("Type:", _assemblyView.label);
                _details.Add(_typeView.label, _typeView.value);

                _isPublicView = BuildLabelView("Is Public:", _typeView.label);
                _details.Add(_isPublicView.label, _isPublicView.value);

                _hasAttributeView = BuildLabelView("Scanable:", _isPublicView.label);
                _details.Add(_hasAttributeView.label, _hasAttributeView.value);

                var conventionsLabel = new Label("Implements Conventions:")
                {
                    X = 1,
                    Y = Pos.Bottom(_hasAttributeView.label),
                    Height = 1,
                    Width = Dim.Fill()
                };
                _conventionsValue = new Label("")
                {
                    X = 2,
                    Y = Pos.Bottom(conventionsLabel),
                    Height = Dim.Fill(),
                    Width = Dim.Fill()
                };
                _details.Add(conventionsLabel, _conventionsValue);

                Add(_assembly.frame, _conventions.frame, _details);

                _assembly.list.SelectedChanged += UpdateAssembly;
                _conventions.list.SelectedChanged += UpdateDetails;
            }

            public sealed override void Add(View view) => base.Add(view);

            public LocateWindow UpdateAssemblies(AssemblyDefinition[] assemblies)
            {
                _assemblies = assemblies;
                _assembly.frame.Width = assemblies.Max(x => x.Name.Length) + 4;
                _assembly.list.SetSource(assemblies);
                UpdateAssembly();
                return this;
            }

            public LocateWindow UpdateDefinitions(ConventionDefinition[] definitions)
            {
                _definitions = definitions;
                _conventions.frame.Width = definitions.Max(x => x.Name.Length) + 4;
                _conventions.list.SetSource(definitions);
                UpdateAssembly();
                return this;
            }

            private void UpdateAssembly()
            {
                if (_assembly.list.Source.Count == 0)
                {
                    UpdateDetails();
                    return;
                }

                var builder = _assemblies[_assembly.list.SelectedItem];
                _conventionDetails = builder.Conventions
                   .Select(
                        assembly => new AssemblyConventionDetails(
                            assembly,
                            _definitions
                               .Where(z => z.ConventionType.IsAssignableFrom(assembly))
                               .ToArray()
                        )
                    )
                   .ToArray();
                _conventions.list.SetSource(_conventionDetails);
                UpdateDetails();
            }

            private void UpdateDetails()
            {
                if (_conventions.list.Source.Count == 0)
                {
                    _assemblyView.value.Text = "";
                    _isPublicView.value.Text = "";
                    _typeView.value.Text = "";
                    _hasAttributeView.value.Text = "";
                    return;
                }

                _conventions.frame.Width = _conventionDetails.Max(x => x.ToString().Length) + 4;
                var detail = _conventionDetails[_conventions.list.SelectedItem];
                _assemblyView.value.Text = MinLabelText(detail.Assembly.GetName().FullName);
                _isPublicView.value.Text = MinLabelText(detail.IsPublic ? "Yes" : "No");
                _hasAttributeView.value.Text = MinLabelText(detail.HasAttribute ? "Yes" : "No");
                _typeView.value.Text = MinLabelText(detail.Type.FullName);
                _conventionsValue.Text = string.Join("\n", detail.Definitions.Select(z => "* " + z.Name));
            }

            private string MinLabelText(string? value) => ( value ?? string.Empty ).Substring(
                0,
                _details.Frame.Width <= 0
                    ? ( value ?? string.Empty ).Length
                    : Math.Min(
                        ( value ?? string.Empty ).Length,
                        _details.Frame.Width - _assemblyView.label.Frame.Width - 4
                    )
            );
        }

        private class CustomWindow : Toplevel
        {
            public override bool ProcessKey(KeyEvent keyEvent)
            {
                if (keyEvent.Key == Key.Esc)
                {
                    Application.RequestStop();
                    return true;
                }

                return base.ProcessKey(keyEvent);
            }
        }
    }

    internal enum ConventionKind
    {
        Convention,
        Delegate
    }

    internal class AssemblyDefinition
    {
        public AssemblyDefinition(Assembly assembly)
        {
            Assembly = assembly;

            Conventions = assembly.DefinedTypes.Where(x => typeof(IConvention).IsAssignableFrom(x) && x.IsClass)
               .ToArray();
        }

        public Assembly Assembly { get; }
        public string Name => Assembly.GetName().Name ?? string.Empty;
        public TypeInfo[] Conventions { get; }

        public override string ToString() => Name;
    }

    internal class AssemblyConventionDetails
    {
        public AssemblyConventionDetails(TypeInfo convention, ConventionDefinition[] definitions)
        {
            Type = convention;
            Definitions = definitions;
            HasAttribute = convention.Assembly.GetCustomAttributes<ConventionAttribute>()
               .Any(x => x.Type.GetTypeInfo() == convention);
            IsPublic = convention.IsPublic;
        }

        public TypeInfo Type { get; }
        public ConventionDefinition[] Definitions { get; }

        public Assembly Assembly => Type.Assembly;
        public bool HasAttribute { get; }
        public bool IsPublic { get; }

        public override string ToString() => Type.Name;
    }

    internal class ConventionDetails
    {
        public ConventionDetails(DelegateOrConvention convention, int index)
        {
            Index = index;
            Kind = convention.Convention == null ? ConventionKind.Delegate : ConventionKind.Convention;
            if (convention.Convention != null)
            {
                Type = convention.Convention.GetType().GetTypeInfo();
                Assembly = Type.Assembly;
                Name = Type.Name;
            }
            else
            {
                Type = convention.Delegate!.Method.DeclaringType!.GetTypeInfo();
                Assembly = Type.Assembly;
                var name = convention.Delegate!.Method.Name;
                var methodType = convention.Delegate.Method.DeclaringType;
                var rootName = methodType!.FullName;

                if (methodType.IsNested)
                {
                    rootName = methodType.DeclaringType!.Name;
                }

                var regex = new Regex("<(\\w+?)>b_+\\d+?_+(\\d+?)", RegexOptions.Compiled);
                var result = regex.Match(name);
                if (result.Success && result.Groups.Count > 1)
                {
                    name = result.Groups[1].Value + "(" + result.Groups[2].Value + ")";
                }

                Name = $"{rootName}+{name}";
            }
        }

        public ConventionKind Kind { get; }
        public Assembly Assembly { get; }
        public TypeInfo Type { get; }

        [UsedImplicitly]
        public string Name { get; }

        [UsedImplicitly]
        public int Index { get; }

        public override string ToString() => $"{Index + 1}) {Name}";
    }

    internal class ConventionDefinition
    {
        public ConventionDefinition(TypeInfo type)
        {
            Type = type;
            var name = type.Name.TrimStart('I');
            if (name.EndsWith("Builder", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.LastIndexOf("Builder", StringComparison.OrdinalIgnoreCase));
            }

            if (name.EndsWith("Convention", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.LastIndexOf("Convention", StringComparison.OrdinalIgnoreCase));
            }

            Name = name;

            var container = type.ImplementedInterfaces.First(
                z => z.IsGenericType && typeof(IConventionContainer<,,>) == z.GetGenericTypeDefinition()
            );
            ConventionType = container.GenericTypeArguments[1].GetTypeInfo();
            DelegateType = container.GenericTypeArguments[2].GetTypeInfo();
        }

        [UsedImplicitly]
        public TypeInfo Type { get; }

        public string Name { get; }
        public TypeInfo ConventionType { get; }
        public TypeInfo DelegateType { get; }
        public override string ToString() => Name;
    }

    /// <summary>
    /// DumpCommand.
    /// </summary>
    [UsedImplicitly]
    [Command("conventions", "convention", Description = "Convention Diagnostics")]
    [Subcommand(typeof(ConventionListCommand))]
    internal class ConventionCommand
    {
        private readonly CommandLineApplication _application;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionCommand" /> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public ConventionCommand(CommandLineApplication application) => _application = application;

        /// <summary>
        /// Called when [execute].
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int OnExecute()
        {
            _application.ShowHelp();
            return 0;
        }
    }

    /// <summary>
    /// A common diagnostics command to attach multiple commands to.
    /// </summary>
    [Command("diagnostics", "diag", Description = "Convention Diagnostics")]
    public class DiagnosticsCommand
    {
        private readonly CommandLineApplication _application;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsCommand" /> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public DiagnosticsCommand(CommandLineApplication application) => _application = application;

        /// <summary>
        /// Called when [execute].
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int OnExecute()
        {
            _application.ShowHelp();
            return 0;
        }
    }
}