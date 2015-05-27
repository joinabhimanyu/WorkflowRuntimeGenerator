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
using System.Xml;
using System.Xml.Linq;


namespace WorkflowRuntimeGenerator
{
    public class EventDrivenActivity
    {
        public string eventname { get; set; }
        public string handleExternal { get; set; }
        public string setState { get; set; }
    }

    public class StateActivity
    {
        public string state_name { get; set; }
        public bool InitialState { get; set; }
        public List<EventDrivenActivity> event_driven { get; set; }

    }

    public class DisplayState
    {
        public string disp_state_name { get; set; }
        public string actual_state_name { get; set; }
        public string completed_state_name { get; set; }
        public bool InitialState { get; set; }
        public List<string> target_states { get; set; }
        public List<string> targetstate_displayname { get; set; }
        public List<string> all_states { get; set; }
    }

    

    public partial class WorkflowModification : System.Web.UI.Page
    {
        //public List<StateActivity> actual_states = null;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                panel_header.Visible = false;
                lblHeader.Text = "Welcome to Workflow Modification Wizard";
                panel_filename.Visible = true;

                if (Request.GetFriendlyUrlSegments().Count > 0)
                {
                    panel_header.Visible = true;
                    panel_filename.Visible = false;

                    IList<string> segments = Request.GetFriendlyUrlSegments();
                    int param_count = Request.GetFriendlyUrlSegments().Count;
                    for (int i = 0; i < param_count; i++)
                    {
                        lblHeader.Text = "File loaded!";
                        lblHeader.Text += @" The file path is 'C:\inetpub\wwwroot\Download\" + segments[i].ToString().Trim() + ".xoml'";
                    }

                }

            }
        }


        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public static string CheckUser(string value)
        {
            string result = string.Format("Welcome {0}", value);

            return result;

        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public static string GenerateFile(string workflow_name)
        {
            string result = "";
            try
            {


                result = "success";
            }
            catch (Exception ex)
            {

                result = ex.Message.ToString().Trim();
            }

            return result;
        }

        private string UpdateGridStates() {

            List<StateActivity> states = null;
            StateActivity state = null;
            List<EventDrivenActivity> eves = null;
            EventDrivenActivity ev = null;

            string result = "";
            string filePath = "";

            if (Request.GetFriendlyUrlSegments().Count > 0)
            {
                IList<string> segments = Request.GetFriendlyUrlSegments();
                filePath = @"C:\inetpub\wwwroot\Download\" + segments[0].ToString().Trim() + ".xoml";
                Session["filename"] = segments[0].ToString().Trim();
            }
            else
            {
                filePath = txtWorkflowName.Text.Trim();
            }

            if (filePath != "")
            {
                Session["filepath"] = filePath.ToString().Trim();
                XDocument doc = XDocument.Load(filePath);
                XNamespace w = "http://schemas.microsoft.com/winfx/2006/xaml/workflow";
                XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";

                states = new List<StateActivity>();
                foreach (XElement element in doc.Root.Elements(w + "StateActivity"))
                {
                    state = new StateActivity();
                    state.state_name = element.Attribute(x + "Name").Value.ToString().Trim();
                    state.InitialState = false;
                    eves = new List<EventDrivenActivity>();
                    foreach (XElement inner_element in element.Elements(w + "EventDrivenActivity"))
                    {
                        ev = new EventDrivenActivity();
                        ev.eventname = inner_element.Attribute(x + "Name").Value.ToString().Trim();
                        ev.handleExternal = inner_element.Element(w + "HandleExternalEventActivity").Attribute("EventName").Value.ToString().Trim();
                        ev.setState = inner_element.Element(w + "SetStateActivity").Attribute("TargetStateName").Value.ToString().Trim();
                        eves.Add(ev);
                        ev = null;
                    }
                    state.event_driven = eves;
                    states.Add(state);
                    eves = null;
                    state = null;

                }

                var start_workflow = from start_state in states where start_state.state_name == "START_WORKFLOW" select start_state.event_driven[0].setState.ToString().Trim();
                string start_workflow_name = start_workflow.First().ToString().Trim();

                foreach (StateActivity each_state in states)
                {
                    if (each_state.state_name == start_workflow_name.ToString().Trim())
                    {
                        each_state.InitialState = true;
                    }
                }

                Session["actual_states"] = states;
                grdStates.DataSource = null;
                grdStates.DataSource = states;
                grdStates.DataBind();

                result = "success";

            }
            else
            {
                result = "error";
            }
            return result;
        }

        protected void btnShowResult_Click(object sender, EventArgs e)
        {
            ShowResultRow();
        }

        private void ShowResultRow()
        {
            string result = UpdateGridStates();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script language='javascript' type='text/javascript'>");
            string str = string.Format(@"ShowResultRow('{0}')", result.ToString().Trim());
            sb.Append(str);
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);
        }

        protected void btnDisplayStateTransition_Click(object sender, EventArgs e)
        {

            string result = "";
            try
            {
                if (Session["filepath"] != null)
                {
                    List<StateActivity> actual_states = (List<StateActivity>)Session["actual_states"];
                    List<DisplayState> dispState = new List<DisplayState>();
                    DisplayState state = null;
                    List<string> targetStates = null;
                    List<string> targetstate_displayname = null;
                    StateActivity tempstate = null;
                    string completed_state = "";
                    string qstring = "";
                    DataRow drow = null;
                    DataObjectClass obj_dataobject = new DataObjectClass();
                    string state_status = "";

                    for (int i = 1; i < actual_states.Count; i++)
                    {
                        state = new DisplayState();

                        qstring = string.Format("select decode(s.txt_state_display_name,'','End_State','Start_State'),s.txt_state_display_name Display_Name" +
                                                " from workflow_state_master s where txt_state_name='{0}'", actual_states[i].state_name.ToString().Trim());

                        drow = obj_dataobject.getSQLDataRow(qstring.Trim());

                        if (drow != null)
                        {
                            state_status = drow[0].ToString().Trim();
                        }

                        if (state_status == "Start_State")
                        {
                            state.actual_state_name = actual_states[i].state_name.ToString().Trim();
                            state.disp_state_name = drow[1].ToString().Trim();
                            drow = null;

                            state.InitialState = actual_states[i].InitialState;
                            completed_state = actual_states[i].event_driven[0].setState.ToString().Trim();
                            state.completed_state_name = completed_state.ToString().Trim();

                            tempstate = (from temp in actual_states where temp.state_name == completed_state select temp).First();

                            targetStates = new List<string>();
                            foreach (EventDrivenActivity ediv in tempstate.event_driven)
                            {
                                targetStates.Add(ediv.setState.ToString().Trim());
                            }
                            state.target_states = targetStates;

                            targetstate_displayname = new List<string>();
                            foreach (string target in targetStates)
                            {
                                qstring = string.Format("select s.txt_state_display_name Display_Name" +
                                                        " from workflow_state_master s where txt_state_name='{0}'", target.Trim());
                                drow = obj_dataobject.getSQLDataRow(qstring.Trim());
                                if (drow != null)
                                {
                                    targetstate_displayname.Add(drow[0].ToString().Trim());
                                }
                            }
                            state.targetstate_displayname = targetstate_displayname;

                            targetstate_displayname = null;
                            targetStates = null;
                            tempstate = null;
                            dispState.Add(state);
                            state = null;
                        }

                    }
                    List<string> all_states = new List<string>();
                    foreach (DisplayState item in dispState)
                    {
                        all_states.Add(item.disp_state_name.ToString().Trim());
                    }

                    foreach (DisplayState item in dispState)
                    {
                        item.all_states = all_states;
                    }

                    Session["display_states"] = dispState;
                    grdStateTransition.DataSource = null;
                    grdStateTransition.DataSource = dispState;
                    grdStateTransition.DataBind();
                    result = "success";
                }
                else
                {
                    result = "Please read the file to proceed";
                }

            }
            catch (Exception ex)
            {

                result = ex.Message.ToString().Trim();
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script language='javascript' type='text/javascript'>");
            string str = string.Format(@"DisplayStateTransition('{0}')", result.ToString().Trim());
            sb.Append(str);
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);

        }

        protected void btnModify_Click(object sender, EventArgs e)
        {
            string result = "";
            List<DisplayState> dispstate = (List<DisplayState>)Session["display_states"];
            try
            {

                foreach (DisplayState item in dispstate)
                {
                    item.target_states.Clear();
                    item.targetstate_displayname.Clear();
                }

                DataObjectClass obj_dataobject = null;
                List<string> source_states = new List<string>();
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
                            source_states.Add(dt.Rows[i].ItemArray[0].ToString());
                        }
                    }

                    lstSourceStates.DataSource = source_states;
                    lstSourceStates.DataBind();


                    result = "success";
                }
                catch (Exception ex)
                {

                    result = ex.Message.ToString().Trim();
                }
                grdStateModification.DataSource = null;
                grdStateModification.DataSource = dispstate;
                grdStateModification.DataBind();
            }
            catch (Exception ex)
            { 

            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script language='javascript' type='text/javascript'>");
            string str = string.Format(@"DisplayStateModification('{0}')", result.ToString().Trim());
            sb.Append(str);
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);
        }

        //protected void btnShowNewStates_Click(object sender, EventArgs e)
        //{
        //    row2.Visible = true;
        //    UpdatePanel2.Update();

        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    sb.Append(@"<script language='javascript' type='text/javascript'>");
        //    string script = "ShowNewStates();";
        //    sb.Append(script);
        //    sb.Append(@"</script>");
        //    ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);
        //}

        protected void chk_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;
            GridViewRow selectedrow = (GridViewRow)chkBox.Parent.Parent;
            int index = selectedrow.DataItemIndex;
            List<DisplayState> dispstate = (List<DisplayState>)Session["display_states"];
            if (chkBox.Checked)
            {
                foreach (DisplayState item in dispstate)
                {
                    item.InitialState = false;
                }
                dispstate[index].InitialState = true;
            }
            else
            {
                dispstate[index].InitialState = false;
            }

            grdStateModification.DataSource = null;
            grdStateModification.DataSource = dispstate;
            grdStateModification.DataBind();
            UpdatePanel2.Update();

        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            string result = "";
            try
            {
                List<DisplayState> dispstate = (List<DisplayState>)Session["display_states"];
                DisplayState temp_state = null;
                string initState = "";
                string filePath = Session["filepath"].ToString().Trim();

                Session["filename"] = filePath.Substring(filePath.LastIndexOf(@"\") + 1).Trim();

                List<string> new_states = new List<string>();
                DataObjectClass obj_dataobject = new DataObjectClass();
                DataTable dtable = new DataTable();

                string qstring = "";
                var display = isRowVisible.Value.ToString().Trim();

                if (display != "false")
                {
                    foreach (ListItem item in lstDestinationStates.Items)
                    {
                        new_states.Add(item.Value.ToString().Trim());
                    }

                    foreach (string item in new_states)
                    {
                        temp_state = new DisplayState();
                        temp_state.disp_state_name = item.ToString().Trim();
                        temp_state.InitialState = false;

                        qstring = string.Format("select k.txt_state_name state,k.txt_state_display_name from workflow_state_master k" +
                            " where k.txt_start_comp_link_id=(select p.txt_start_comp_link_id from workflow_state_master p where p.txt_state_display_name='{0}')" +
                            " order by k.txt_state_id", temp_state.disp_state_name.ToString().Trim());

                        dtable = obj_dataobject.getSQLDataTable(qstring);
                        if (dtable != null && dtable.Rows.Count > 0)
                        {
                            temp_state.actual_state_name = dtable.Rows[0][0].ToString().Trim();
                            temp_state.completed_state_name = dtable.Rows[1][0].ToString().Trim();
                        }

                        dispstate.Add(temp_state);
                        temp_state = null;
                    }

                    List<string> every_state = new List<string>();

                    foreach (DisplayState item in dispstate)
                    {
                        every_state.Add(item.disp_state_name.ToString().Trim());
                    }

                    foreach (DisplayState item in dispstate)
                    {
                        item.all_states = null;
                        item.all_states = new List<string>();
                        item.all_states = every_state;
                        item.target_states = new List<string>();
                        item.targetstate_displayname = new List<string>();
                    }

                    Session["display_states"] = dispstate;

                    grdStateModification.DataSource = null;
                    grdStateModification.DataSource = dispstate;
                    grdStateModification.DataBind();
                    isRowVisible.Value = "false";
                    UpdatePanel2.Update();

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(@"<script language='javascript' type='text/javascript'>");
                    string script = "HideAllStates();";
                    sb.Append(script);
                    sb.Append(@"</script>");
                    ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);

                }
                else
                {
                    if (filePath != null)
                    {

                        XDocument doc = XDocument.Load(filePath);
                        XNamespace w = "http://schemas.microsoft.com/winfx/2006/xaml/workflow";
                        XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";

                        initState = dispstate.Where(state => state.InitialState == true).Select(state => state.actual_state_name).First().ToString().Trim();

                        foreach (XElement element in doc.Root.Elements(w + "StateActivity"))
                        {
                            if (element.Attribute(x + "Name").Value.ToString().Trim() == "START_WORKFLOW")
                            {
                                foreach (XElement inner_element in element.Elements(w + "EventDrivenActivity"))
                                {
                                    inner_element.Attribute(x + "Name").Value = "ON_START_WORKFLOW_" + initState.ToString().Trim();
                                    inner_element.Element(w + "HandleExternalEventActivity").Attribute(x + "Name").Value = "START_WORKFLOW_" + initState.ToString().Trim();
                                    inner_element.Element(w + "HandleExternalEventActivity").Attribute("EventName").Value = initState.ToString().Trim();
                                    inner_element.Element(w + "SetStateActivity").Attribute("TargetStateName").Value = initState.ToString().Trim();
                                    inner_element.Element(w + "SetStateActivity").Attribute(x + "Name").Value = "setStateActivity_START_WORKFLOW_" + initState.ToString().Trim();
                                }
                            }
                            //else
                            //{
                            //    element.RemoveAll();
                            //}
                        }
                        doc.Save(filePath);


                        //foreach (XNode node in doc.Root.Nodes())
                        //{
                        //    node.NodesAfterSelf().Remove();
                        //}

                        doc.Root.Descendants(w + "StateActivity")
                            .Where(node => node.Attribute(x + "Name").Value.ToString().Trim() != "START_WORKFLOW")
                            .Remove();

                        doc.Save(filePath);

                        StringBuilder str = new StringBuilder();
                        List<string> targetStates = new List<string>();

                        str.Append("<StateMachineWorkflowActivity x:Class=" + "\"" + "WorkflowLibrary1." + Session["filename"].ToString().Trim() + "\"" + " InitialStateName=" + "\"" + "START_WORKFLOW" + "\"" + " x:Name=" + "\"" + Session["filename"].ToString().Trim() + "\"" + " DynamicUpdateCondition=" + "\"" + "{x:Null}" + "\"" + " CompletedStateName=" + "\"" + "{x:Null}" + "\"" + " xmlns:x=" + "\"" + "http://schemas.microsoft.com/winfx/2006/xaml" + "\"" + " xmlns=" + "\"" + "http://schemas.microsoft.com/winfx/2006/xaml/workflow" + "\"" + " > ");

                        foreach (DisplayState state in dispstate)
                        {
                            targetStates.Add(GetEndStateName(state.actual_state_name.ToString().Trim()));
                            str.Append(CreateStateForMarkup(GetStartStateName(state.actual_state_name.ToString().Trim()), targetStates));

                            str.Append(CreateStateForMarkup(targetStates.First().ToString().Trim(), state.target_states));
                            targetStates.Clear();
                        }

                        str.Append("</StateMachineWorkflowActivity>");

                        XDocument ndoc = XDocument.Parse(str.ToString().Trim());

                        foreach (XNode node in ndoc.Root.Nodes())
                        {
                            doc.Root.Add(node);
                            doc.Save(filePath);
                        }

                        doc = null;
                        str.Clear();
                        result = "success";
                    }

                    //ShowResultRow();
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(@"<script language='javascript' type='text/javascript'>");
                    string script = string.Format(@"SaveConfirmation('{0}')", result.ToString().Trim());
                    sb.Append(script);
                    sb.Append(@"</script>");
                    ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "none", sb.ToString(), false);

                }
                     
            }
            catch (Exception ex)
            {

                result = ex.Message.ToString().Trim();
            }

        }

        protected void chkSelectedLinks_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckBoxList chkBoxList = (CheckBoxList)sender;
            GridViewRow selectedrow = (GridViewRow)chkBoxList.Parent.Parent;
            int index = selectedrow.DataItemIndex;
            List<DisplayState> dispstate = (List<DisplayState>)Session["display_states"];
            string value = "";
            string target_state = "";
            foreach (ListItem selected_item in chkBoxList.Items)
            {

                value = selected_item.Value.ToString().Trim();
                target_state = (from state in dispstate where state.disp_state_name == value.Trim() select state.actual_state_name.ToString().Trim()).First().ToString().Trim();


                if (selected_item.Selected == true)
                {
                    if (!dispstate[index].targetstate_displayname.Contains(value))
                    {
                        dispstate[index].targetstate_displayname.Add(value);              
                        dispstate[index].target_states.Add(target_state);
                    }
                }
                else
                {
                    if (dispstate[index].targetstate_displayname.Contains(selected_item.Value.ToString().Trim()))
                    {
                        dispstate[index].targetstate_displayname.Remove(selected_item.Value.ToString().Trim());
                        dispstate[index].target_states.Remove(target_state);
                    }                  
                }
            }
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
            string strQuery = "select s.txt_state_name Start_State from workflow_state_master s where s.txt_state_name='" + strStateDispName + "' ";
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
            string strQuery = "select t.txt_state_name End_State from workflow_state_master t where t.txt_state_id = (select max(to_number(a.txt_state_id)) " + "from workflow_state_master a where a.txt_start_comp_link_id in (select s.txt_start_comp_link_id from workflow_state_master s " + "where s.txt_state_name = '" + strStateDispName + "'))";
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

        protected void lnkCloseModify_Click(object sender, EventArgs e)
        {
            ShowResultRow();
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

    }
}