using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using IMsftConfigurationBuilder = Microsoft.Extensions.Configuration.IConfigurationBuilder;

namespace Rocket.Surgery.Extensions.Configuration
{
    /// <summary>
    /// Options for configuring a hosting environment
    /// </summary>
    public class ConfigOptions
    {
        /// <summary>
        /// Additional settings providers to be inserted after the default application settings file (typically appsettings.json)
        /// </summary>
        public List<Func<IMsftConfigurationBuilder, IMsftConfigurationBuilder>> ApplicationConfiguration { get; } =
            new List<Func<IMsftConfigurationBuilder, IMsftConfigurationBuilder>>()
            {
                b => b.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true),
                b => b.AddYamlFile("appsettings.yml", optional: true, reloadOnChange: true),
                b => b.AddYamlFile("appsettings.yaml", optional: true, reloadOnChange: true),
                b => b.AddIniFile("appsettings.ini", optional: true, reloadOnChange: true)
            };

        /// <summary>
        /// Additional settings providers to be inserted after the default environment application settings file (typically
        /// appsettings.{env}.json)
        /// </summary>
        public List<Func<IMsftConfigurationBuilder, string, IMsftConfigurationBuilder>> EnvironmentConfiguration
        {
            get;
        } =
            new List<Func<IMsftConfigurationBuilder, string, IMsftConfigurationBuilder>>()
            {
                (b, environmentName) => b.AddJsonFile(
                    $"appsettings.{environmentName}.json",
                    optional: true,
                    reloadOnChange: true
                ),
                (b, environmentName) => b.AddYamlFile(
                    $"appsettings.{environmentName}.yml",
                    optional: true,
                    reloadOnChange: true
                ),
                (b, environmentName) => b.AddYamlFile(
                    $"appsettings.{environmentName}.yaml",
                    optional: true,
                    reloadOnChange: true
                ),
                (b, environmentName) => b.AddIniFile(
                    $"appsettings.{environmentName}.ini",
                    optional: true,
                    reloadOnChange: true
                )
            };
    }
}