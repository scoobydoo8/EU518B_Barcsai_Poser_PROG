namespace ParentalControl.Data
{
    using ParentalControl.Data.Database;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Database Extension class.
    /// </summary>
    public static class DatabaseManagerExtension
    {
        /// <summary>
        /// Create.
        /// </summary>
        /// <typeparam name="T">T.</typeparam>
        /// <param name="table">Table.</param>
        /// <param name="item">Item.</param>
        public static void Create<T>(this DbSet<T> table, T item)
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
        public static List<T> Read<T>(this DbSet<T> table, Func<T, bool> condition)
            where T : class
        {
            return table.Where(x => condition == null || condition(x)).ToList();
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <typeparam name="T">T.</typeparam>
        /// <param name="table">Table.</param>
        /// <param name="condition">Condition.</param>
        /// <param name="action">Action.</param>
        public static void Update<T>(this DbSet<T> table, Func<T, bool> condition, Action<T> action)
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
        public static void Delete<T>(this DbSet<T> table, Func<T, bool> condition)
            where T : class
        {
            table.RemoveRange(table.Where(x => condition == null || condition(x)));
        }
    }
}
