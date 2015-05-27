<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkflowModification.aspx.cs" Inherits="WorkflowRuntimeGenerator.WorkflowModification" %>

<%@ Import Namespace="System.Web.Services" %>
<%@ Import Namespace="System.Web.Script.Services" %>
<%@ Import Namespace="Microsoft.AspNet.FriendlyUrls" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <title>Page 3</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <link rel="stylesheet" type="text/css" href="Content/bootstrap.min.css" />
    <link rel="stylesheet" type="text/css" href="Content/alertify.core.css" />
    <link rel="stylesheet" type="text/css" href="Content/alertify.default.css" />
    <link rel="stylesheet" type="text/css" href="bower_components/sweetalert/lib/sweet-alert.css" />

    <link href='http://fonts.googleapis.com/css?family=Indie+Flower' rel='stylesheet' type='text/css' />
    <link href='http://fonts.googleapis.com/css?family=Droid+Serif:400,700' rel='stylesheet' type='text/css' />
    <link href='http://fonts.googleapis.com/css?family=Lobster' rel='stylesheet' type='text/css' />

    <link rel="stylesheet" type="text/css" href="Content/uikit.min.css" />
    <link rel="stylesheet" type="text/css" href="Content/uikit.gradient.min.css" />
    <link rel="stylesheet" type="text/css" href="Content/uikit.almost-flat.min.css" />


    <style type="text/css">
        body {
            background-color: white;
            background-position: center;
            background-repeat: no-repeat;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
        }

        .container {
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            position: absolute;
        }

        #row2 {
            display: none;
        }

        #page-one {
            z-index: 10;
            display: block;
        }

        #entry-row, #result-row {
            margin-top: 10px;
        }

        #result-row {
            height: auto;
        }

        #txtWorkflowName {
            margin-top: 7px;
        }

        #btnSubmit {
            margin-top: 6px;
        }

        #lblWorkflowName {
            font-family: 'Open Sans Condensed', sans-serif;
            font-size: 44px;
            -webkit-transition: all 0.3s linear;
            -moz-transition: all 0.3s linear;
            -o-transition: all 0.3s linear;
            transition: all 0.3s linear;
        }

        /*@font-face {
            font-family: myFont;
            src: url('~/fonts/Sansation_Light.ttf');
        }*/
        #btnReadFile, #btnDisplayStateTransition, #btnModify, #btnSaveChanges, #btnShowNewStates {
            font-family: 'Droid Serif', serif;
            font-size: 1.4em;
            color: white;
        }

        p {
            font-weight: bold;
            font-family: 'Indie Flower', cursive;
        }

        h1 {
            font-family: 'Droid Serif', serif;
            /*font-family: 'Lobster', cursive;*/
            font-size: 2.5em;
            color: white;
        }

        h3 {
            font-family: 'Droid Serif', serif;
            /*font-family: myFont;*/
            font-size: 2.5em;
            color: black;
        }

        #lblHeader {
            font-family: 'Open Sans Condensed', sans-serif;
            font-size: 2.1em;
            color: black;
        }

        #listTargetStates {
            width: 200px;
            overflow-x: auto;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">

        <asp:ScriptManager ID="asm" runat="server" EnablePartialRendering="true" EnablePageMethods="true">

            <Scripts>

                <asp:ScriptReference Path="~/Scripts/jquery-1.10.2.min.js" />
                <asp:ScriptReference Path="~/Scripts/alertify.js" />
                <asp:ScriptReference Path="~/Scripts/bootstrap.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery.velocity.min.js" />
                <asp:ScriptReference Path="~/Scripts/velocity.ui.js" />
                <asp:ScriptReference Path="~/bower_components/sweetalert/lib/sweet-alert.min.js" />
                <asp:ScriptReference Path="~/Scripts/uikit.min.js" />

            </Scripts>

        </asp:ScriptManager>

        <div class="container" id="page-one">

            <div class="row" id="entry-row">
                <div class="col-md-12">
                    <div class="panel panel-success">
                        <div class="panel-heading">
                            <h1>Workflow Modification Wizard</h1>
                        </div>
                        <div class="panel-body">

                            <asp:Panel ID="panel_header" runat="server" CssClass="well">
                                <asp:Label ID="lblHeader" CssClass="control-label" runat="server"></asp:Label>
                            </asp:Panel>

                            <asp:Panel ID="panel_filename" runat="server" CssClass="form-group">
                                <asp:TextBox ID="txtWorkflowName" runat="server" CssClass="form-control"></asp:TextBox>
                            </asp:Panel>

                            <ul class="list-inline">
                                <li>
                                    <asp:Button ID="btnReadFile" runat="server" CssClass="btn btn-primary" Text="Read File" OnClick="btnShowResult_Click" />
                                </li>
                                <li>
                                    <asp:Button ID="btnDisplayStateTransition" runat="server" CssClass="btn btn-primary" Text="Display in State Transition Pattern" OnClick="btnDisplayStateTransition_Click" />
                                </li>
                            </ul>

                        </div>
                    </div>

                </div>
            </div>

            <div class="row" id="result-row">
                <div class="col-md-12">
                    <div class="panel">
                        <div class="panel-body">
                            <asp:UpdatePanel ID="updStates" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate>

                                    <asp:GridView ID="grdStates" runat="server" CssClass="table table-bordered" AutoGenerateColumns="false" ItemType="WorkflowRuntimeGenerator.StateActivity">
                                        <Columns>

                                            <asp:CheckBoxField HeaderText="Is Initial" DataField="InitialState" />

                                            <asp:TemplateField HeaderText="State Activity Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblStateName" runat="server" Text='<%# Eval("state_name") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="EventDriven Activity Details">
                                                <ItemTemplate>

                                                    <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered" AutoGenerateColumns="false" ItemType="WorkflowRuntimeGenerator.EventDrivenActivity" DataSource='<%# Eval("event_driven") %>'>
                                                        <Columns>
                                                            <%--<asp:TemplateField HeaderText="Event Name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblEventName" runat="server" Text='<%# Eval("eventname") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>--%>

                                                            <asp:TemplateField HeaderText="HandleExternal Event Name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblHandleEvent" runat="server" Text='<%# Eval("handleExternal") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="SetState Activity Name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSetState" runat="server" Text='<%# Eval("setState") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>

                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>

                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnReadFile" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="lnkCloseModify" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <!-- Modal -->
        <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                        <h4 class="modal-title" id="myModalLabel">The Philosophy</h4>
                    </div>
                    <div class="modal-body">
                        <div class="well">
                            <h3>Well this is my first attempt to merge the two worlds of HTML5 and ASP.NET Web Forms and I can say that this feels more awesome than it sounds to be. Glad you came by and had a look.</h3>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>

                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="modalStates" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                        <h4 class="modal-title" id="H1">Generated States</h4>
                    </div>
                    <div class="modal-body">
                        <div class="well">

                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate>
                                    <asp:GridView ID="grdStateTransition" runat="server" CssClass="table table-bordered table-responsive" AutoGenerateColumns="false" ItemType="WorkflowRuntimeGenerator.DisplayState">
                                        <Columns>

                                            <asp:CheckBoxField HeaderText="Is Initial" DataField="InitialState" />

                                            <asp:TemplateField HeaderText="State Name" ControlStyle-Width="100%">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblDisplayName" Text='<%# Eval("disp_state_name") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Target State Name">
                                                <ItemTemplate>
                                                    <asp:ListBox ID="listTargetStates" runat="server" CssClass="form-control" DataSource='<%# Eval("targetstate_displayname") %>' Enabled="False"></asp:ListBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnDisplayStateTransition" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnModify" runat="server" CssClass="btn btn-primary" Text="Modify Transition" OnClick="btnModify_Click" />
                    </div>
                </div>
            </div>
        </div>



        <div class="modal fade" id="modalModify" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog" style="width: 70%;">
                <div class="modal-content">
                    <div class="modal-header">

                        <%--<button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>--%>
                        
                        <asp:LinkButton ID="lnkCloseModify" runat="server" CssClass="close" OnClientClick="return ToggleModifyModal();" OnClick="lnkCloseModify_Click">
                            <span aria-hidden="true">&times;</span><span class="sr-only">Close</span>
                        </asp:LinkButton>

                        <h4 class="modal-title" id="H2">State Modification</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Panel CssClass="row" ID="row1" runat="server">
                            <div class="well">

                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate>
                                    <div class="row well">
                                        <asp:HiddenField ID="isRowVisible" runat="server" Value="false" />
                                        <asp:GridView ID="grdStateModification" runat="server" CssClass="table table-bordered table-responsive" AutoGenerateColumns="false" ItemType="WorkflowRuntimeGenerator.DisplayState">
                                            <Columns>

                                                <asp:TemplateField HeaderText="Is Initial State">
                                                    <ItemTemplate>
                                                        <asp:RadioButton ID="chk" runat="server" Checked='<%# Eval("InitialState") %>' AutoPostBack="true" OnCheckedChanged="chk_CheckedChanged" />
                                                    </ItemTemplate>

                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="State Name" ControlStyle-Width="100%">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblDisplayName" Text='<%# Eval("disp_state_name") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Target States">
                                                    <ItemTemplate>
                                                        <asp:CheckBoxList ID="chkSelectedLinks" runat="server" AutoPostBack="true" Height="23px" Width="300px" Visible="true" DataSource='<%# Eval("all_states") %>' OnSelectedIndexChanged="chkSelectedLinks_SelectedIndexChanged">
                                                        </asp:CheckBoxList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>

                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnModify" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="btnSaveChanges" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                        </asp:Panel>

                        <asp:Panel runat="server" CssClass="row" ID="row2">
                            <div class="well">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <div class="row">
                                <div class="col-md-5">
                                    <div class="panel panel-primary" style="width: 350px;">
                                        <div class="panel-heading">
                                            <h2 class="header">Available states</h2>
                                        </div>
                                        <div class="panel-body">
                                    
                                    <asp:ListBox ID="lstSourceStates" CssClass="form-control" runat="server" Height="200px" Width="200px" SelectionMode="Multiple"></asp:ListBox>

                                        </div>
                                    </div>
                                    
                                </div>
                                
                                <div class="col-md-1">
                                    <p>
                                        <asp:Button ID="InsertSelection" runat="server" CssClass="btn btn-success" Enabled="true" Text="Insert" OnClick="InsertSelection_Click" />
                                    </p>
                                    <p>
                                        <asp:Button ID="DeleteSelection" runat="server" CssClass="btn btn-warning" Enabled="true" Text="Delete" OnClick="DeleteSelection_Click" />
                                    </p>
                                </div>
                        
                                <div class="col-md-4">
                                    <div class="panel panel-primary" style="width: 350px;">
                                        <div class="panel-heading">
                                            <h2 class="header">Selected workflow states</h2>
                                        </div>
                                        <div class="panel-body">
                                    
                                    <asp:ListBox ID="lstDestinationStates" CssClass="form-control" runat="server" Height="200px" Width="200px" SelectionMode="Multiple"></asp:ListBox>

                                        </div>
                                    </div>
                                    
                                </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnModify" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveChanges" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="InsertSelection" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="DeleteSelection" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
                            </div>
                        </asp:Panel>
                        
                    </div>
                    <div class="modal-footer">
                        <div class="row well">
                            <ul class="list-inline">
                                <li>
                                    <asp:Button ID="btnSaveChanges" runat="server" CssClass="btn btn-primary" Text="Save Changes" OnClick="btnSaveChanges_Click" />
                                </li>
                                <li>
                                    <button id="btnShowNewStates" class="btn btn-primary">Add States</button>
                                   <%-- <asp:Button ID="btnShowNewStates" runat="server" CssClass="btn btn-primary" Text="Add States" OnClick="btnShowNewStates_Click" />--%>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>



    </form>

    <script type="text/javascript">

        function selectionError() {
            alertify.error("Please select workflow states to insert");
        }

        //function ShowNewStates() {
        //    $("#row1").velocity("transition.slideDownOut", 500);
        //    $("#row2").velocity("transition.slideUpIn", 1350);
        //}

        function HideAllStates() {
            $("#row2").velocity("transition.slideDownOut", 500);
            $("#row1").velocity("transition.slideUpIn", 1350);
        }

        function RemoveError() {
            alertify.error("Please select workflow states to delete");
        }

        function ToggleModal() {
            $("#modalStates").modal('toggle');
        }

        function ToggleModifyModal() {
            $("#modalModify").modal('toggle');
            return true;
        }

        function SaveConfirmation(result) {
            if (result == "success") {
                
                sweetAlert('Great!', 'Changes saved successfully. Please close the modal to refresh the page', 'success');
            }
            else {
                alertify.error(result);
            }
        }

        function DisplayStateModification(result) {
            if (result == "success") {
                $("#modalStates").modal('toggle');
                $("#modalModify").modal('toggle');
            }
        }

        function ShowResultRow(result) {
            if (result == "success") {
                $("#result-row").velocity("transition.slideUpIn", 1250);
            }
            else {
                alertify.alert('Please enter file name');
            }
        }

        function DisplayStateTransition(result) {
            if (result == "success") {
                $("#modalStates").modal('toggle');
            }
            else {
                sweetAlert('Oops!', result, 'error');
            }
        }

        $(document).ready(function () {

            $("#result-row").css("display", "none");

            $("#txtWorkflowName").attr("placeholder", "Enter the file path if different from default path");

            $("#btnShowNewStates").click(function (e) {
                e.preventDefault();
                $("#row1").velocity("transition.slideDownOut", 500);
                $("#row2").velocity("transition.slideUpIn", 1350);
                $("#isRowVisible").attr('value', 'true');
            });

            //$("#btnSubmit").click(function (e) {
            //    e.preventDefault();
            //    var value = $("#txtBox").val();
            //    if (value == "abhi") {
            //        $.ajax({
            //            url: 'http://localhost/WebFormsDemo/WebForm3.aspx/CheckUser',
            //            type: 'POST',
            //            data: ' { value:"' + value + '" }',
            //            dataType: 'json',
            //            contentType: 'application/json; charset=utf-8',
            //            success: function (data) {

            //                $("#label1").html(data.d);
            //                $("#result-row").velocity("transition.flipBounceYIn", 1250);
            //                $("#add-row").velocity("transition.flipBounceYIn", 2250);

            //            },
            //            error: function (response) {
            //                var r = jQuery.parseJSON(response.responseText);
            //                alertify.alert("Message: " + r.Message);
            //                alertify.alert("StackTrace: " + r.StackTrace);
            //                alertify.alert("ExceptionType: " + r.ExceptionType);
            //            }
            //        });
            //    }
            //    else {
            //        alertify.error("You cannot go further");
            //    }

            //});

            //$("#btnGenerateFile").click(function (e) {
            //    e.preventDefault();
            //    var workflowName = $("#txtWorkflowName").val();

            //    $.ajax({
            //        url: 'http://localhost/WebFormsDemo/WebForm3.aspx/GenerateFile',
            //        type: 'POST',
            //        data: ' { workflow_name:"' + workflowName + '" }',
            //        dataType: 'json',
            //        contentType: 'application/json; charset=utf-8',
            //        success: function (data) {

            //            var result = data.d;
            //            if (result == "success") {
            //                sweetAlert('Great!', 'File generated successfully', 'success');
            //            }
            //            else {
            //                sweetAlert('Oops!', 'Something went wrong', 'error');
            //            }
            //            //alert(data.d);
            //        },
            //        error: function (response) {
            //            var r = jQuery.parseJSON(response.responseText);
            //            alertify.alert("Message: " + r.Message);
            //            alertify.alert("StackTrace: " + r.StackTrace);
            //            alertify.alert("ExceptionType: " + r.ExceptionType);
            //        }
            //    });
            //});




        });

    </script>

</body>
</html>
