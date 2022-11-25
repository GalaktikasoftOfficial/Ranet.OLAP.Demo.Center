using System;
using System.Linq;
using System.Windows.Threading;
using Ranet.AgOlap.Controls.General;
using Ranet.AgOlap.Controls.PivotEditorForm.Contract;
using Ranet.AgOlap.Controls.PivotEditorForm.Contract.Args;
using Ranet.AgOlap.Controls.PivotEditorForm.PivotEditorControl;
using Ranet.AgOlap.Services;
using Ranet.Olap.Core.Common;
using Ranet.Olap.Core.Helpers;
using Ranet.Olap.Core.PivotEditor;
using Ranet.Olap.Core.Types;
using Zaaml.UI;
using Zaaml.UI.Windows;


namespace Ranet.DemoCenter
{
	public partial class PivotEditorTest
	{
		#region Ctors

		public PivotEditorTest()
		{
			InitializeComponent();


			var connectionString = "Provider=MSOLAP.4;DATA SOURCE=by01-1048;CATALOG=Sample01;";
			var cubeName = "MyPerspective";
			var setColumns = "[Product].[Unique Name].Levels ( 1 ).Members";
			var setRows = "Head([Period].[Date].Levels ( 1 ).Members, 10)";
			var updateScript = @"
(
IIF(<%newValue%> < 0, [Measures].[Customer Count], [Measures].[Summa])
, <%[Period].[Date]%>
, <%[Product].[Unique Name]%>
) = <%newValue%>
";


			var settings = new PivotEditorSettingsControl(cubeName, connectionString, setColumns, setRows)
			{
				MdxCommandProvider = new PivotEditorDataService(connectionString, cubeName),
				FuncNewOlapDataLoader = p => RanetServiceLocator.GetDataLoader(),
				SubCube = new SubSelectProvider(""),
				LogService = new DefaultLogManager(Dispatcher),
				TemplateUpdateScript = updateScript,
				CanModifyColumns = true,
				CanModifyRows = true,
				CalculatedMembers = Enumerable.Empty<CalculatedMemberDesign>()
			};
			var pivotEditorControl = new PivotEditorCntrl(settings, null);

			pivotEditorControl.ShowRecalculateButton = true;
			pivotEditorControl.Initialization();

			LayoutRoot.Children.Add(pivotEditorControl);
		}

		#endregion
	}

	internal class DefaultUIService : IUIService
	{
		#region Fields

		private readonly Dispatcher _dispatcher;

		#endregion

		#region Ctors

		public DefaultUIService(Dispatcher dispatcher)
		{
			_dispatcher = dispatcher;
		}

		#endregion

		#region  Methods

		public void ShowError(Exception exp)
		{
			ShowMessageWindow(exp.Message, "Error");
		}

		public void ShowError(string message)
		{
			ShowMessageWindow(message, "Error");
		}

		public void ShowError(string caption, string message)
		{
			ShowMessageWindow(message, caption);
		}

		public void ShowInformation(string caption, string text)
		{
			ShowMessageWindow(text, caption);
		}

		public void ShowInformation(string text)
		{
			ShowMessageWindow(text, "Information");
		}

		public void ShowInformation(string text, Action<bool?> action)
		{
			ShowMessageWindow(text, "Information", action);
		}

		public void ShowInformation(string caption, string text, string advancedMessage)
		{
			ShowMessageWindow(string.Format("{0}\n{1}", text, advancedMessage), caption);
		}

		public void ShowQuestion(string caption, string text, Action<bool?> action)
		{
			ShowMessageWindow(text, caption, action);
		}

		public void ShowQuestion(string text, Action<bool?> action)
		{
			ShowMessageWindow(text, "Question", action);
		}

		public void ShowWarning(string caption, string text)
		{
			ShowMessageWindow(text, caption);
		}

		public void ShowWarning(string text)
		{
			ShowMessageWindow(text, "Warning");
		}

		public void ShowWarning(string text, Action<bool?> action)
		{
			ShowMessageWindow(text, "Warning", action);
		}

		private static bool? ConvertResult(MessageWindowResultKind resultKind)
		{
			switch (resultKind)
			{
				case MessageWindowResultKind.None:
					return null;
				case MessageWindowResultKind.OK:
					return true;
				case MessageWindowResultKind.Cancel:
					return false;
				case MessageWindowResultKind.Yes:
					return true;
				case MessageWindowResultKind.No:
					return false;
				default:
					throw new ArgumentOutOfRangeException("resultKind");
			}
		}

		private void ShowMessageWindow(string text, string caption, Action<bool?> action = null)
		{
			if (_dispatcher.CheckAccess() == false)
			{
				_dispatcher.BeginInvoke(() => ShowMessageWindow(text, caption, action));
				return;
			}

			if (action != null)
				MessageWindow.Show(text, caption, MessageWindowButtons.OKCancel, r => action(ConvertResult(r.Result)));
			else
				MessageWindow.Show(text, caption, MessageWindowButtons.OK, r => { });
		}

		#endregion
	}

	internal class OlapDataLoaderEx : OlapDataLoader
	{
	}

	internal class PivotEditorDataService : OlapDataLoaderEx, IMdxCommandProvider
	{
		#region Fields

		private readonly string _connectionString;
		private readonly string _cubeName;

		public event EventHandler<ExecuteQueryResultEventArgs> ExecuteSelectCompleted = (sender, args) => { };
		public event EventHandler<MdxCommandResultEventArgs> ExecuteUpdateCompleted = (sender, args) => { };
		public event EventHandler<GetCubeMetadataResultEventArgs> GetCubeMetadataCompleted = (sender, args) => { };

		#endregion

		#region Ctors

		public PivotEditorDataService(string connectionString, string cubeName)
		{
			_connectionString = connectionString;
			_cubeName = cubeName;
		}

		#endregion

		#region  Methods

		private void OnExecuteSelectCompleted(string result, string sessionId, Exception exception, UserState userState)
		{
			if (exception != null)
				ExecuteSelectCompleted(this, new ExecuteQueryResultEventArgs(exception, userState.ExternalUserState, sessionId));
			else
				ExecuteSelectCompleted(this, new ExecuteQueryResultEventArgs(result, userState.ExternalUserState, sessionId));
		}

		private void OnExecuteUpdateCompleted(string result, string sessionId, Exception exception, UserState userState)
		{
			if (exception != null)
				ExecuteUpdateCompleted(this, new MdxCommandResultEventArgs(exception, userState.ExternalUserState, sessionId));
			else
				ExecuteUpdateCompleted(this, new MdxCommandResultEventArgs(userState.ExternalUserState, sessionId));
		}

		public void ExecuteSelect(string query, object state)
		{
			ExecuteSelect(_connectionString, query, state);
		}

		public void ExecuteSelect(string connectionString, string query, object state)
		{
			LoadData(this, CommandHelper.CreateMdxQueryArgs(connectionString, query), new UserState { ExternalUserState = state, QueryType = QueryType.Select });
		}

		public void ExecuteUpdate(string query, object state)
		{
			ExecuteUpdate(_connectionString, query, state);
		}

		public void ExecuteUpdate(string connectionString, string query, object state)
		{
			var args = new MdxQueryArgs
			{
				Connection = connectionString,
				Type = QueryTypes.Update,
				Query = query
			};

			LoadData(this, args, new UserState { ExternalUserState = state, QueryType = QueryType.Update });
		}

		protected override void OnPerformOlapServiceActionCompleted(object sender, object userState, InvokeResultDescriptor result, Exception exception)
		{
			var myUserState = (UserState)userState;
			var actualException = exception;
			if (result.ContentType == InvokeContentType.Error && actualException == null)
				actualException = new Exception(result.Content);

			var sessionIdHeader = result.Headers.Contains(InvokeResultDescriptor.SESSION_ID) ? result.Headers[InvokeResultDescriptor.SESSION_ID] : null;
			var sessionId = sessionIdHeader != null ? sessionIdHeader.Value : null;

			var actualResult = result.Content;

			switch (myUserState.QueryType)
			{
				case QueryType.Update:
					OnExecuteUpdateCompleted(actualResult, sessionId, actualException, myUserState);
					break;
				case QueryType.Select:
					OnExecuteSelectCompleted(actualResult, sessionId, actualException, myUserState);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void GetCubeMetadata(object state)
		{
			throw new NotImplementedException();
		}

		public void GetCubeMetadata(string connectionString, string cubeName, object state)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region  Nested Types

		private enum QueryType
		{
			Update,
			Select,
			Metadata
		}

		private class UserState
		{
			#region Fields

			public object ExternalUserState;
			public QueryType QueryType;

			#endregion
		}

		#endregion
	}
}
