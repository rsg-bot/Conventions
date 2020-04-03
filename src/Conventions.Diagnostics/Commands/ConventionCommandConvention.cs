using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.Linq;
using System.Threading.Tasks;
using Rocket.Surgery.Extensions.CommandLine;

namespace Rocket.Surgery.Conventions.Diagnostics.Commands
{
    internal class ConventionCommandConvention : ICliConvention
    {
        public void Register(ICliConventionContext context)
        {
            var diagnosticsCommand = context.Builder.Command.Children.GetByAlias("diagnostics") as Command;
            if (diagnosticsCommand == null)
            {
                diagnosticsCommand = new Command("diagnostics", "Application Diagnostics");
                context.Builder.AddCommand(diagnosticsCommand);
            }
            if (!diagnosticsCommand.HasAlias("diag"))
            {
                diagnosticsCommand.AddAlias("diag");
            }
            
            var conventionsCommand = diagnosticsCommand.Children.GetByAlias("diagnostics") as Command;
            if (conventionsCommand == null)
            {
                conventionsCommand = new Command("conventions", "Convention Diagnostics");
                diagnosticsCommand.AddCommand(conventionsCommand);
            }
            if (!conventionsCommand.HasAlias("convention"))
            {
                conventionsCommand.AddAlias("convention");
            }

            var listCommand = new Command(
                "list",
                "Applies all outstanding changes to the database based on the current configuration"
            );
            conventionsCommand.AddCommand(listCommand);
            // listCommand.Handler = CommandHandler.Create();
            // // To make this work...
            // HandlerDescriptor.FromMethodInfo(
            //     // The method info (onExecute, execute, returning int/task<int>/task/void/,
            //     // the instance created from dependenncy injection
            //     ).GetCommandHandler()
                
            

            // diagnosticsCommand.Handler = CommandHandler.Create();
        }

        class CommandHandler<T> : ICommandHandler
        {
            public CommandHandler()
            {
                
            }
            public Task<int> InvokeAsync(InvocationContext context)
            {
                context.
            }
        }
    }
}