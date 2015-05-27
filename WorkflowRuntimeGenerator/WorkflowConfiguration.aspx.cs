using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using DbUtility;
using System.Data;
using System.Text;
using System.IO;
using System.Net;
using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;


namespace WorkflowRuntimeGenerator
{
    public partial class WorkflowConfiguration : System.Web.UI.Page
    {

        //private data member declarations
        string strFilePath = null;
        private static Dictionary<string, string> dicSelectedStateDtls = new Dictionary<string, string>();
        private static Dictionary<string, List<string>> dicStateTransitionDtls = new Dictionary<string, List<string>>();


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.GetFriendlyUrlSegments().Count() > 0)
                {
                    IList<string> segments = Request.GetFriendlyUrlSegments();

                }
                
            }
            
        }


        protected void btnSelectState_Click(object sender, EventArgs e)
        {
            DataObjectClass obj_dataobject = null;
            List<string> result = new List<string>();
            DataTable dt = new DataTable();
            lstSourceStates.Items.Clear();
            lstDestinationStates.Items.Clear();
            try
            {
                obj_dataobject = new DataObjectClass();

                string qstring = "select Distinct k.txt_state_display_name" +
                                 " from workflow_state_master k" +
                                 " where k.txt_state_display_name is not null" +
                                 " order by k.txt_state_display_name";

                dt = obj_dataobject.getSQLDataTable(qstring);
                if (dt != null || dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        result.Add(dt.Rows[i].ItemArray[0].ToString());
                    }
                }

                lstSourceStates.DataSource = result;
                lstSourceStates.DataBind();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script language='javascript' type='text/javascript'>");
                sb.Append(@"showSelection();");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);

            }
            catch (Exception)
            {

                result.Add("Error occurred");
            }

        }

        protected void lstSourceStates_SelectedIndexChanged(object sender, EventArgs e)
        {
            int[] ar = lstSourceStates.GetSelectedIndices();
            
            if (ar.Length > 0)
            {
                InsertSelection.Enabled = true;
            }
            else
            {
                InsertSelection.Enabled = false;
            }
        }

        protected void InsertSelection_Click(object sender, EventArgs e)
        {
            lstDestinationStates.DataSource = null;
            List<string> result = new List<string>();
            int[] ar = lstSourceStates.GetSelectedIndices();
            if (ar.Length > 0)
            {
                for (int i = 0; i < ar.Length; i++)
                {
                    result.Add(lstSourceStates.Items[ar[i]].ToString().Trim());
                }
                lstDestinationStates.DataSource = result;
                lstDestinationStates.DataBind();
            }
            else
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script language='javascript' type='text/javascript'>");
                sb.Append(@"selectionError();");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);
            }
            
        }

        protected void DeleteSelection_Click(object sender, EventArgs e)
        {
            int[] ar = lstDestinationStates.GetSelectedIndices();
            if (ar.Length > 0)
            {
                List<string> bound = new List<string>();
                for (int i = 0; i < ar.Length; i++)
                {
                    bound.Add(lstDestinationStates.Items[ar[i]].Value.ToString().Trim());
                }
                for (int i = 0; i < bound.Count; i++)
                {
                    lstDestinationStates.Items.Remove(bound[i].ToString().Trim());
                    lstDestinationStates.DataBind();
                }
                UpdatePanel1.Update();
            }
            else
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script language='javascript' type='text/javascript'>");
                sb.Append(@"RemoveError();");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);
            }
        }


        protected void CreateFile_Click(object sender, EventArgs e)
        {
            string result = "";
            try
            {
                //string destPath = @"C:\Users\u3696174\Documents\Visual Studio 2012\Projects\WebFormsDemo\WebFormsDemo\Downloaded";
                string destPath = System.Configuration.ConfigurationManager.AppSettings["destFilePath"].ToString().Trim();
                string destFile = Path.Combine(destPath.Trim(), txtWorkflowName.Text.Trim() + ".xoml");

                if (!Directory.Exists(txtPath.Text.Trim()))
                {
                    Directory.CreateDirectory(txtPath.Text.Trim());
                }

                if (!Directory.Exists(destPath.Trim()))
                {
                    Directory.CreateDirectory(destPath.Trim());
                }
                else
                {
                    string[] files = Directory.GetFiles(destPath.Trim());
                    if (files.Length > 0)
                    {
                        foreach (string s in files)
                        {
                            File.Delete(s);
                        }
                    }
                    Directory.Delete(destPath.Trim());
                    Directory.CreateDirectory(destPath.Trim());
                }

                strFilePath = Path.Combine(txtPath.Text.Trim(), txtWorkflowName.Text.Trim());
                StreamWriter strwrtWfMrkup = new StreamWriter(strFilePath + ".xoml");
                StreamWriter destWriter = new StreamWriter(destFile.Trim());

                strwrtWfMrkup.WriteLine("<StateMachineWorkflowActivity x:Class=" + "\"" + "WorkflowLibrary1." + txtWorkflowName.Text.Trim() + "\"" + " InitialStateName=" + "\"" + "START_WORKFLOW" + "\"" + " x:Name=" + "\"" + txtWorkflowName.Text.Trim() + "\"" + " DynamicUpdateCondition=" + "\"" + "{x:Null}" + "\"" + " CompletedStateName=" + "\"" + "{x:Null}" + "\"" + " xmlns:x=" + "\"" + "http://schemas.microsoft.com/winfx/2006/xaml" + "\"" + " xmlns=" + "\"" + "http://schemas.microsoft.com/winfx/2006/xaml/workflow" + "\"" + " > ");
                destWriter.WriteLine("<StateMachineWorkflowActivity x:Class=" + "\"" + "WorkflowLibrary1." + txtWorkflowName.Text.Trim() + "\"" + " InitialStateName=" + "\"" + "START_WORKFLOW" + "\"" + " x:Name=" + "\"" + txtWorkflowName.Text.Trim() + "\"" + " DynamicUpdateCondition=" + "\"" + "{x:Null}" + "\"" + " CompletedStateName=" + "\"" + "{x:Null}" + "\"" + " xmlns:x=" + "\"" + "http://schemas.microsoft.com/winfx/2006/xaml" + "\"" + " xmlns=" + "\"" + "http://schemas.microsoft.com/winfx/2006/xaml/workflow" + "\"" + " > ");

                List<string> lstActualDestInitStateName = new List<string>();
                for (int Count = 0; Count <= rdbSetInitStates.Items.Count - 1; Count++)
                {
                    if (rdbSetInitStates.Items[Count].Selected)
                    {
                        lstActualDestInitStateName.Add(GetStartStateName(rdbSetInitStates.Items[Count].ToString()));
                    }
                }
                strwrtWfMrkup.Write(CreateStateForMarkup("START_WORKFLOW", lstActualDestInitStateName));
                destWriter.Write(CreateStateForMarkup("START_WORKFLOW", lstActualDestInitStateName));

                foreach (object Item_loopVariable in dicStateTransitionDtls.Keys)
                {
                    List<string> lstActualDestStateName = new List<string>();
                    lstActualDestStateName.Add(GetEndStateName(Item_loopVariable.ToString()));
                    strwrtWfMrkup.Write(CreateStateForMarkup(GetStartStateName(Item_loopVariable.ToString()), lstActualDestStateName));
                    destWriter.Write(CreateStateForMarkup(GetStartStateName(Item_loopVariable.ToString()), lstActualDestStateName));
                }

                foreach (object Item_loopVariable in dicStateTransitionDtls.Keys)
                {
                    List<string> lstDestStateName = new List<string>();
                    List<string> lstActualDestStateName = new List<string>();
                    lstDestStateName = dicStateTransitionDtls[Item_loopVariable.ToString()];
                    foreach (object DestState_loopVariable in lstDestStateName)
                    {
                        lstActualDestStateName.Add(GetStartStateName(DestState_loopVariable.ToString()));
                    }
                    strwrtWfMrkup.Write(CreateStateForMarkup(GetEndStateName(Item_loopVariable.ToString()), lstActualDestStateName));
                    destWriter.Write(CreateStateForMarkup(GetEndStateName(Item_loopVariable.ToString()), lstActualDestStateName));
                }

                foreach (object Item_loopVariable in dicSelectedStateDtls.Keys)
                {
                    if (!dicStateTransitionDtls.ContainsKey(Item_loopVariable.ToString()))
                    {
                        List<string> lstActualDestStateName = new List<string>();
                        lstActualDestStateName.Add(GetEndStateName(Item_loopVariable.ToString()));
                        strwrtWfMrkup.Write(CreateStateForMarkup(GetStartStateName(Item_loopVariable.ToString()), lstActualDestStateName));
                        lstActualDestStateName = null;
                        strwrtWfMrkup.Write(CreateStateForMarkup(GetEndStateName(Item_loopVariable.ToString()), lstActualDestStateName));
                        destWriter.Write(CreateStateForMarkup(GetEndStateName(Item_loopVariable.ToString()), lstActualDestStateName));
                    }
                }

                strwrtWfMrkup.WriteLine("</StateMachineWorkflowActivity>");
                destWriter.WriteLine("</StateMachineWorkflowActivity>");
                strwrtWfMrkup.Dispose();
                destWriter.Dispose();

                result = "success";

                //File.Copy(txtPath.Text.Trim(), destFile.Trim(), true);
                
            }
            catch (Exception)
            {
                result = "Error occurred";
            }

            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // `Dns.Resolve()` method is deprecated.
            //IPAddress ipAddress = ipHostInfo.AddressList[0];

            //string fileName = "http://" + ipAddress.ToString() + "/WebFormsDemo/Images/" + txtWorkflowName.Text.Trim() + ".xoml";
            string fileName = txtWorkflowName.Text.Trim() + ".xoml";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script language='javascript' type='text/javascript'>");
            string str = string.Format(@"CheckFileCreate('{0}', '{1}')", result.ToString().Trim(), fileName.Trim());

            sb.Append(str);
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);

        }

        //State Activity Markup creation

        private string CreateStateForMarkup(string strStateName, List<string> StrTargetStateName)
        {
            StringBuilder strbuilder = new StringBuilder();
            strbuilder.AppendLine("<StateActivity x:Name=" + "\"" + strStateName + "\"" + ">");
            if (StrTargetStateName != null)
	        {
                foreach (var TargetState in StrTargetStateName)
                {
                    strbuilder.AppendLine("<EventDrivenActivity x:Name=" + "\"" + "ON_" + strStateName + "_" + TargetState + "\"" + ">");
                    strbuilder.AppendLine("<HandleExternalEventActivity x:Name=" + "\"" + strStateName + "_" + TargetState + "\"" + " EventName=" + "\"" + TargetState + "\"" + " InterfaceType=" + "\"" + "{x:Type p8:IEvents}" + "\"" + " xmlns:p8=" + "\"" + "clr-namespace:WorkFlowClassLibrary;Assembly=WorkFlowClassLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" + "\"" + "  />");
                    strbuilder.AppendLine("<SetStateActivity x:Name=" + "\"" + "setStateActivity_" + strStateName + "_" + TargetState + "\"" + " TargetStateName=" + "\"" + TargetState + "\"" + " />");
                    strbuilder.AppendLine("</EventDrivenActivity>");
                }
	        }
            strbuilder.Append("</StateActivity>");
            return strbuilder.ToString();
        }


        //Retrieving Start State Name for each activity

        private string GetStartStateName(string strStateDispName)
        {

            string strStartStateName = string.Empty;
            string strQuery = "select s.txt_state_name Start_State from workflow_state_master s where s.txt_state_display_name='" + strStateDispName + "' ";
            DataObjectClass objDataObjectClass = new DataObjectClass();
            DataTable dtWorkflow = new DataTable();
            try
            {
                dtWorkflow = objDataObjectClass.getSQLDataTable(strQuery);
                if ((dtWorkflow != null) && dtWorkflow.Rows.Count > 0)
                {
                    strStartStateName = dtWorkflow.Rows[0][0].ToString();
                }
                return strStartStateName;
            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {
                objDataObjectClass = null;
                dtWorkflow = null;
            }

        }


        //Retrieving End State Name for each activity

        private string GetEndStateName(string strStateDispName)
        {

            string strEndStateName = string.Empty;
            string strQuery = "select t.txt_state_name End_State from workflow_state_master t where t.txt_state_id = (select max(to_number(a.txt_state_id)) " + "from workflow_state_master a where a.txt_start_comp_link_id in (select s.txt_start_comp_link_id from workflow_state_master s " + "where s.txt_state_display_name = '" + strStateDispName + "'))";
            DataObjectClass objDataObjectClass = new DataObjectClass();
            DataTable dtWorkflow = new DataTable();
            try
            {
                dtWorkflow = objDataObjectClass.getSQLDataTable(strQuery);
                if ((dtWorkflow != null) && dtWorkflow.Rows.Count > 0)
                {
                    strEndStateName = dtWorkflow.Rows[0][0].ToString();
                }
                return strEndStateName;
            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {
                objDataObjectClass = null;
                dtWorkflow = null;
            }

        }


        protected void chk_Transition_CheckedChanged(object sender, EventArgs e)
        {
            string strQuery = string.Empty;
            string strCollSelectedLnks = string.Empty;
            DataObjectClass objDataObjectClass = new DataObjectClass();
            DataTable dtWorkflow = new DataTable();
            dicSelectedStateDtls = new Dictionary<string, string>();
            List<string> newList = new List<string>();


            for (int Count = 0; Count <= lstDestinationStates.Items.Count - 1; Count++)
            {
                strCollSelectedLnks += "'" + lstDestinationStates.Items[Count].ToString().Trim() + "',";
            }
            strCollSelectedLnks = strCollSelectedLnks.Trim(',');
            strQuery = "select P.TXT_STATE_ID,P.TXT_START_COMP_LINK_ID,P.TXT_STATE_NAME,P.TXT_STATE_DISPLAY_NAME  FROM WORKFLOW_STATE_MASTER P " + "where P.TXT_START_COMP_LINK_ID IN (SELECT K.TXT_START_COMP_LINK_ID FROM WORKFLOW_STATE_MASTER K WHERE K.TXT_STATE_DISPLAY_NAME IN (" + strCollSelectedLnks + ")) GROUP BY P.TXT_START_COMP_LINK_ID,P.TXT_STATE_ID,P.TXT_STATE_NAME,P.TXT_STATE_DISPLAY_NAME " + "ORDER BY TO_NUMBER(P.TXT_STATE_ID)";

            try
            {
                string firstArg = string.Empty;
                dtWorkflow = objDataObjectClass.getSQLDataTable(strQuery);
                if ((dtWorkflow != null) && dtWorkflow.Rows.Count > 0)
                {
                    for (int Count = 0; Count <= dtWorkflow.Rows.Count - 1; Count++)
                    {
                        if (Count != 0)
                        {
                            if (dtWorkflow.Rows[Count][3] != System.DBNull.Value)
                            {
                                if (dtWorkflow.Rows[Count][3] == System.DBNull.Value)
                                {
                                    firstArg = dtWorkflow.Rows[Count - 1][3].ToString();
                                }
                                else
	                            {
                                    firstArg = dtWorkflow.Rows[Count][3].ToString();
	                            }

                                dicSelectedStateDtls.Add(firstArg, (dtWorkflow.Rows[Count][2] + "," + dtWorkflow.Rows[Count + 1][2]));
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            dicSelectedStateDtls.Add(dtWorkflow.Rows[Count][3].ToString(), dtWorkflow.Rows[Count][2].ToString() + "," + dtWorkflow.Rows[Count + 1][2].ToString());
                        }
                    }
                }

                Session["DICLNKDTLS"] = dicSelectedStateDtls;

                //if (chk_Transition.Checked)
                //{
                //    ddlDisplayLinks.Visible = true;
                //    chkSelectedLinks.Visible = true;
                //    chkSetInitStates.Visible = true;
                ddlDisplayLinks.Items.Clear();
                chkSelectedLinks.Items.Clear();

                for (int i = 0; i <= dicSelectedStateDtls.Count - 1; i++)
                {
                    ddlDisplayLinks.Items.Add(dicSelectedStateDtls.Keys.ElementAt(i).ToString().Trim());
                }
                foreach (object Item_loopVariable in dicSelectedStateDtls.Keys)
                {
                    chkSelectedLinks.Items.Add(Item_loopVariable.ToString());
                }
                //    foreach (object Item_loopVariable in dicSelectedStateDtls.Keys)
                //    {
                //        chkSetInitStates.Items.Add(Item_loopVariable.ToString());
                //    }
                //}
                //else
                //{
                //    ddlDisplayLinks.Visible = false;
                //    chkSelectedLinks.Visible = false;
                //    chkSetInitStates.Visible = false;
                //}
                rdbSetInitStates.Items.Clear();
                foreach (object Item_loopVariable in dicSelectedStateDtls.Keys)
                {
                    rdbSetInitStates.Items.Add(Item_loopVariable.ToString());
                }

                UpdatePanel3.Update();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script language='javascript' type='text/javascript'>");
                sb.Append(@"ShowStateTransitionPanel();");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);

            }
            catch (Exception ex)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script language='javascript' type='text/javascript'>");
                string str = string.Format(@"ShowError('{0}')", ex.Message.ToString().Trim());

                sb.Append(str);
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);
            }

        }


        private void SaveChanges()
        {
            string strSourceState = null;
            List<string> strDestinationState = new List<string>();
            strSourceState = ddlDisplayLinks.SelectedItem.Value;
            for (int Count = 0; Count <= chkSelectedLinks.Items.Count - 1; Count++)
            {
                if (chkSelectedLinks.Items[Count].Selected)
                {
                    strDestinationState.Add(chkSelectedLinks.Items[Count].ToString());
                }
            }

            if (!dicStateTransitionDtls.ContainsKey(strSourceState))
            {
                dicStateTransitionDtls.Add(strSourceState, strDestinationState);
            }

        }

        protected void btnSetInitStates_Click(object sender, EventArgs e)
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script language='javascript' type='text/javascript'>");
            sb.Append(@"ShowStateInitPanel();");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);

        }

        protected void btn_SaveChanges_Click(object sender, EventArgs e)
        {
            SaveChanges();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script language='javascript' type='text/javascript'>");
            sb.Append(@"ChangeSaved();");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);
        }

        protected void ddlDisplayLinks_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkSelectedLinks.ClearSelection();
            UpdatePanel2.Update();
        }


    }
}