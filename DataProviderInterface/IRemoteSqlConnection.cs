﻿using System;
using System.Data;
using System.ServiceModel;

namespace ProductiveRage.SqlProxyAndReplay.DataProviderInterface
{
	[ServiceContract]
	public interface IRemoteSqlConnection
	{
		[OperationContract]
		Guid GetNewId();

		[OperationContract]
		string GetConnectionString(Guid connectionId);
		[OperationContract]
		void SetConnectionString(Guid connectionId, string value);
		[OperationContract]
		int GetConnectionTimeout(Guid connectionId);
		[OperationContract]
		string GetDatabase(Guid connectionId);
		[OperationContract]
		ConnectionState GetState(Guid connectionId);

		[OperationContract]
		void ChangeDatabase(Guid connectionId, string databaseName);

		[OperationContract]
		void Open(Guid connectionId);
		[OperationContract]
		void Close(Guid connectionId);
		[OperationContract]
		void Dispose(Guid connectionId);

		[OperationContract(Name = "BeginTransactionWithDefaultIsolationLevel")]
		Guid BeginTransaction(Guid connectionId);
		[OperationContract]
		Guid BeginTransaction(Guid connectionId, IsolationLevel il);
	}
}