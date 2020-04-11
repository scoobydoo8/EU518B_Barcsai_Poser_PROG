// <copyright file="DatabaseManagerExtension.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.Data.Database;

    /// <summary>
    /// Database Extension class.
    /// </summary>
    internal static class DatabaseManagerExtension
    {
        /// <summary>
        /// Create.
        /// </summary>
        /// <typeparam name="T">T.</typeparam>
        /// <param name="table">Table.</param>
        /// <param name="item">Item.</param>
        /// <exception cref="ArgumentNullException">If table or item is null.</exception>
        internal static void Create<T>(this DbSet<T> table, T item)
            where T : class
        {
            if (item != null)
            {
                table.Add(item);
            }
            else
            {
                throw new ArgumentNullException(nameof(item));
            }
        }

        /// <summary>
        /// Read.
        /// </summary>
        /// <typeparam name="T">T.</typeparam>
        /// <param name="table">Table.</param>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        /// <exception cref="ArgumentNullException">If table is null.</exception>
        internal static List<T> Read<T>(this DbSet<T> table, Func<T, bool> condition)
            where T : class
        {
            return table.Where(x => condition == null || condition(x)).ToList();
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <typeparam name="T">T.</typeparam>
        /// <param name="table">Table.</param>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        /// <exception cref="ArgumentNullException">If table or action is null.</exception>
        internal static void Update<T>(this DbSet<T> table, Action<T> action, Func<T, bool> condition)
            where T : class
        {
            if (action != null)
            {
                foreach (T record in table.Where(x => condition == null || condition(x)))
                {
                    action(record);
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(action));
            }
        }

        /// <summary>
        /// Delete.
        /// </summary>
        /// <typeparam name="T">T.</typeparam>
        /// <param name="table">Table.</param>
        /// <param name="condition">Condition.</param>
        /// <exception cref="ArgumentNullException">If table is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If Where returns out of range..</exception>
        /// <exception cref="ArgumentException">Because of RemoveRange.</exception>
        internal static void Delete<T>(this DbSet<T> table, Func<T, bool> condition)
            where T : class
        {
            table.RemoveRange(table.Where(x => condition == null || condition(x)));
        }
    }
}
