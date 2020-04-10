using ParentalControl.Data.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentalControl.Data
{
    public class DatabaseManager
    {
        private static DatabaseManager _databaseManager = null;
        private ParentalControlEntities entities;
        private DbContextTransaction transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager"/> class.
        /// </summary>
        private DatabaseManager()
        {
            this.entities = new ParentalControlEntities();
            this.transaction = null;
        }

        /// <summary>
        /// Singleton.
        /// </summary>
        /// <returns>DatabaseManager.</returns>
        public static DatabaseManager Get()
        {
            if (_databaseManager == null)
            {
                _databaseManager = new DatabaseManager();
            }

            return _databaseManager;
        }

        /// <summary>
        /// If rollback is nessesery.
        /// </summary>
        public void BeginTransaction()
        {
            this.transaction = this.entities.Database.BeginTransaction();
        }

        /// <summary>
        /// Commit changes.
        /// </summary>
        public void Commit()
        {
            this.transaction.Commit();
            this.transaction.Dispose();
            this.transaction = null;
        }

        /// <summary>
        /// Rollback changes.
        /// </summary>
        public void Rollback()
        {
            this.transaction.Rollback();
            this.transaction.Dispose();
            this.transaction = null;
        }


    }
}
