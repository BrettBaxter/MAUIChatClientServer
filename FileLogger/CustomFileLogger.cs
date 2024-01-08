/// By Brett Baxter & Henderson Bare
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
using System.Xml.Linq;

namespace FileLogger
{
    /// <summary>
    /// Creates and appends a log message to a file.
    /// </summary>
    public class CustomFileLogger : ILogger
    {
        private readonly string _fileName;
        private readonly string _categoryName;

        /// <summary>
        /// Constructor
        /// </summary>
        public CustomFileLogger(string categoryName)
        {
            _categoryName = categoryName;
            _fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + Path.DirectorySeparatorChar
                + $"CS3500-{categoryName}.log";
        }

        /// <summary>
        /// Not required to be implemented for this assignment.
        /// When called create another level of nesting, events produced within the scope
        /// can be tagged with information about where they came from. This can add another
        /// level of specificity to log messages.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not required to be implemented for this assignment.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a log message: DATE - eventID - State - LogLevel - Message
        /// and appends it to the file.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string message = $"Time: {DateTime.Now}---{eventId}---{logLevel}---{state}{Environment.NewLine}";
            File.AppendAllText(_fileName, message);
        }
    }
}
