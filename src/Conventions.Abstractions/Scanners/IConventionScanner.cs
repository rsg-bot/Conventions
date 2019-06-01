﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// The convention scanner interface is used to find conventions
    ///     and return those conventions in order they are added.
    /// </summary>
    public interface IConventionScanner
    {
        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        IConventionScanner AppendConvention(params IConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>The scanner</returns>
        IConventionScanner AppendConvention(IEnumerable<IConvention> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        IConventionScanner PrependConvention(params IConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>The scanner</returns>
        IConventionScanner PrependConvention(IEnumerable<IConvention> conventions);

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>The scanner</returns>
        IConventionScanner PrependDelegate(params Delegate[] delegates);

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>The scanner</returns>
        IConventionScanner PrependDelegate(IEnumerable<Delegate> delegates);


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>The scanner</returns>
        IConventionScanner AppendDelegate(params Delegate[] delegates);

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>The scanner</returns>
        IConventionScanner AppendDelegate(IEnumerable<Delegate> delegates);

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="types">The additional types to exclude.</param>
        /// <returns>The scanner</returns>
        IConventionScanner ExceptConvention(params Type[] types);

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="types">The convention types to exclude.</param>
        /// <returns>The scanner</returns>
        IConventionScanner ExceptConvention(IEnumerable<Type> types);

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="assemblies">The additional types to exclude.</param>
        /// <returns>The scanner</returns>
        IConventionScanner ExceptConvention(params Assembly[] assemblies);

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="assemblies">The convention types to exclude.</param>
        /// <returns>The scanner</returns>
        IConventionScanner ExceptConvention(IEnumerable<Assembly> assemblies);

        /// <summary>
        /// Creates a provider that returns a set of convetions.
        /// </summary>
        /// <returns></returns>
        IConventionProvider BuildProvider();
    }
}
