﻿using System;
using System.Data;

namespace ProductiveRage.SqlProxyAndReplay.DataProviderService.ProxyImplementations.Replay
{
	public sealed class SqlReplayerConnection : IDbConnection
	{
		private readonly Func<QueryCriteria, IDataReader> _dataRetriever;
		private readonly Func<QueryCriteria, Tuple<object>> _scalarDataRetriever;
		private readonly Func<QueryCriteria, int?> _nonQueryRowCountDataRetriever;
		public SqlReplayerConnection(
			Func<QueryCriteria, IDataReader> dataRetriever,
			Func<QueryCriteria, Tuple<object>> scalarDataRetriever,
			Func<QueryCriteria, int?> nonQueryRowCountDataRetriever)
		{
			if (dataRetriever == null)
				throw new ArgumentNullException(nameof(dataRetriever));
			if (scalarDataRetriever == null)
				throw new ArgumentNullException(nameof(scalarDataRetriever));
			if (nonQueryRowCountDataRetriever == null)
				throw new ArgumentNullException(nameof(nonQueryRowCountDataRetriever));

			_dataRetriever = dataRetriever;
			_scalarDataRetriever = scalarDataRetriever;
			_nonQueryRowCountDataRetriever = nonQueryRowCountDataRetriever;
		}

		public string ConnectionString { get; set; }

		/// <summary>
		/// This shouldn't be relevant when returning data from cache somewhere, so just return a default value
		/// </summary>
		public int ConnectionTimeout { get { return 15; } }

		/// <summary>
		/// This also shouldn't be relevant when returning data from cache somewhere, so just return a default value (could try to parse it out
		/// of the connection string but we have no knowledge what type of database that the connection string is for and so there would be no
		/// reliable way to do so)
		/// </summary>
		public string Database { get { return "*SQLProxyAndReplay-Replayer*"; } }

		/// <summary>
		/// For the same reason as why the Database property returns a default value, this can't reliably be implemented and so it throws
		/// </summary>
		public void ChangeDatabase(string databaseName)
		{
			throw new NotImplementedException($"When communicating with a {Database} endpoint, the ChangeDatabase method is not available");
		}

		public ConnectionState State
		{
			get
			{
				// We'll just act as if this connection is always open - the only way that this should be a problem for calling code is if it
				// tries to close a connection and then check that it has successfully closed, which doesn't feel like a common pattern (if it
				// turns out to be more common than expected then some state tracking could be introduced here - start Closed, change to Open
				// if the Open method is called, change to Closed if the Closed method is called, throw exceptions if operations are attempted
				// that require an open connection)
				return ConnectionState.Open;
			}
		}

		public SqlReplayerTransaction BeginTransaction(IsolationLevel il = IsolationLevel.ReadCommitted) { return new SqlReplayerTransaction(this, il); }
		IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il) { return BeginTransaction(il); }
		IDbTransaction IDbConnection.BeginTransaction() { return BeginTransaction(); }

		public void Open() { }
		public void Close() { }
		public void Dispose() { }

		public SqlReplayerCommand CreateCommand() { return new SqlReplayerCommand(this, _dataRetriever, _scalarDataRetriever, _nonQueryRowCountDataRetriever); }
		IDbCommand IDbConnection.CreateCommand() { return CreateCommand(); }
	}
}