/// By Henderson Bare & Brett Baxter
/// Created: 3/22/2023
/// Course: CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Henderson Bare - This work may not be copied for use in academic course work
/// 
/// I, Henderson Bare and Brett Baxter, certify that I wrote this code from scratch and did not copy it in part or in whole from another source.
/// All references used in the completion of the assignment are cited in my README files and commented in the code.

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLogger
{
    /// <summary>
    /// A wrapper class for the CustomFileLogger. Creates and returns a new CustomFileLogger.
    /// </summary>
    public class CustomFileLogProvider : ILoggerProvider
    {
        CustomFileLogger? _logger;
        /// <summary>
        /// Creates and returns a new CustomFileLogger
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            _logger = new CustomFileLogger(categoryName);
            return _logger;

        }

        /// <summary>
        /// Closes the logger's file.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
