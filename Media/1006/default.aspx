<%@ Page Language="C#" MasterPageFile="~/ShiptalkWeb.Master"  AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ShiptalkWeb._Default" Title="Shiptalk" %>

<%--<asp:Content id="Content1" contentplaceholderid="head" runat="server" />--%>
<asp:Content ID="contentHead" ContentPlaceHolderID="head" runat="server">
  <META http-equiv="refresh" content="0;URL=https://shipnpr.acl.gov/Default.aspx"> 
    <meta http-equiv="content-type" content="text/html;charset=ISO-8859-1" />
                <meta name="description"
                  content="The State Health Insurance Assistance Program, or SHIP, is a national program that offers one-on-one counseling and assistance to people with Medicare and their families" />
</asp:Content>



<asp:Content id="Content2" contentplaceholderid="body1" runat="server">

    <div id="maincontent">
        <div class="dv3col">
            <h1>What is SHIP?</h1>
            <p>The State Health Insurance Assistance Program, or SHIP, is a 
            national program that offers one-on-one counseling and assistance 
            to people with Medicare and their families.</p>
        </div>
        <div class="clear"></div>
        
        
        <div class="dv4colleft dvFindAShip">
            <h2>Find a State SHIP</h2>
            <p>Looking for a State SHIP?  Select your state below to find your local SHIP branch.</p>
            <asp:DropDownList runat="server" ID="ddlStates1" DataTextField="Value" DataValueField="Key"
                                                            AppendDataBoundItems="true" Width="170px" ToolTip="Select State" 
                                onselectedindexchanged="ddlStates1_SelectedIndexChanged">
                                                            <asp:ListItem Text="Select a State" Value="0" />
                                                        </asp:DropDownList>
            <asp:Button ID="Button2" runat="server" Text="GO >>" CssClass="formbutton3" />
        </div>
        
        <div class="dv4colright dvFindACounselor">
            <h2>Find a Counselor</h2>
            <p>Looking for a Ship Counselor?  Select your State and County below.</p>
             <asp:DropDownList runat="server"  ID="ddlStates" DataTextField="Value" DataValueField="Key"
                                                            AppendDataBoundItems="true" Width="170px"  ToolTip="Select State" 
                                onselectedindexchanged="ddlStates_SelectedIndexChanged" AutoPostBack="True">
                                                            <asp:ListItem Text="Select a State" Value="0" />
                                                        </asp:DropDownList>
                                                        
                                                            <asp:DropDownList runat="server" ID="ddlCounties" DataTextField="Value" DataValueField="Key"
                                                            AppendDataBoundItems="true" Width="170px"  ToolTip="Select County" 
                                onselectedindexchanged="ddlCounties_SelectedIndexChanged">
                                                            <asp:ListItem Text="Select a County" Value="0" />
                                                        </asp:DropDownList>
           
            <asp:Button ID="Button3" OnClick="Button3_Click" runat="server" Text="GO >>" CssClass="formbutton3" />
        </div>
        <div class="clear"></div>
        
       
    </div>
    
</asp:Content>
