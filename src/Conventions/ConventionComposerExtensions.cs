﻿using System;
using System.Linq;

namespace Rocket.Surgery.Conventions
{
    public static class ConventionComposerExtensions
    {
        /// <summary>
        /// Uses all the conventions and calls the register method for all of them.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="context">The valid context for the types</param>
        /// <param name="type">The first type to compose with.  This type will either be a <see cref="Delegate"/> that takes <see cref="IConventionContext"/>, or a type that implements <see cref="IConvention{IConventionContext}"/></param>
        /// <param name="types">The other types to compose with.  This type will either be a <see cref="Delegate"/> that takes <see cref="IConventionContext"/>, or a type that implements <see cref="IConvention{IConventionContext}"/></param>
        public static void Register(this IConventionComposer composer, IConventionContext context, Type type, params Type[] types)
        {
            composer.Register(context, new[] { type }.Concat(types));
        }
    }
}