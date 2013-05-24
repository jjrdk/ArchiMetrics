namespace ArchiMeter.Tfs
{
	using System;
	using System.Data.SqlClient;

	using ArchiMeter.Common;

	public class SqlConnectionFactory : IFactory<SqlConnection>
	{
		private readonly string _connectionString;

		public SqlConnectionFactory(string connectionString)
		{
			_connectionString = connectionString;
		}

		public SqlConnection Create()
		{
			return new SqlConnection(_connectionString);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if(isDisposing)
			{
			}
		}

		~SqlConnectionFactory()
		{
			// Simply call Dispose(false).
			this.Dispose(false);
		}

	}
}