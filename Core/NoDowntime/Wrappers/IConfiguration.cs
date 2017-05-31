using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDowntime.Wrappers
{
    internal interface IConfiguration
    {
        //
        // Summary:
        //     Gets the System.Configuration.AppSettingsSection data for the current application's
        //     default configuration.
        //
        // Returns:
        //     Returns a System.Collections.Specialized.NameValueCollection object that contains
        //     the contents of the System.Configuration.AppSettingsSection object for the current
        //     application's default configuration.
        //
        // Exceptions:
        //   T:System.Configuration.ConfigurationErrorsException:
        //     Could not retrieve a System.Collections.Specialized.NameValueCollection object
        //     with the application settings data.
        NameValueCollection AppSettings { get; }
        //
        // Summary:
        //     Gets the System.Configuration.ConnectionStringsSection data for the current application's
        //     default configuration.
        //
        // Returns:
        //     Returns a System.Configuration.ConnectionStringSettingsCollection object that
        //     contains the contents of the System.Configuration.ConnectionStringsSection object
        //     for the current application's default configuration.
        //
        // Exceptions:
        //   T:System.Configuration.ConfigurationErrorsException:
        //     Could not retrieve a System.Configuration.ConnectionStringSettingsCollection
        //     object.
        ConnectionStringSettingsCollection ConnectionStrings { get; }

        //
        // Summary:
        //     Retrieves a specified configuration section for the current application's default
        //     configuration.
        //
        // Parameters:
        //   sectionName:
        //     The configuration section path and name.
        //
        // Returns:
        //     The specified System.Configuration.ConfigurationSection object, or null if the
        //     section does not exist.
        //
        // Exceptions:
        //   T:System.Configuration.ConfigurationErrorsException:
        //     A configuration file could not be loaded.
        object GetSection(string sectionName);
        //
        // Summary:
        //     Refreshes the named section so the next time that it is retrieved it will be
        //     re-read from disk.
        //
        // Parameters:
        //   sectionName:
        //     The configuration section name or the configuration path and section name of
        //     the section to refresh.
        void RefreshSection(string sectionName);
    }
}
