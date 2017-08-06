<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Explore.aspx.cs" Inherits="HelloWorld.Explore.Explore" Async="true" AsyncTimeout="600000" EnableEventValidation = "false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style ="padding: 20px;text-align:center">
        <span style="display: block; overflow: hidden; padding-right:10px;">
            <asp:TextBox ID="txtExplore" runat="server" Width="1500px"></asp:TextBox>
        </span>
        <span>
            <asp:Label ID="lblInfo" runat="server" Text="* Paste TripAdvisor url (e.g https://www.tripadvisor.com/Tourism-g1758900-Taguig_City_Metro_Manila_Luzon-Vacations.html)"></asp:Label>
        </span>       
    </div>
    <div style="text-align:center">
        <asp:Button ID="btnExplore" runat="server" Text="Explore Taguig" OnClick="btnExplore_Click" />
        <asp:Button ID="btnExploreCustom" runat="server" Text="Explore Custom" OnClick="btnExploreCustom_Click" />
    </div >
    <div style ="padding: 20px">
        <asp:GridView ID="gvExplore" runat="server" OnSelectedIndexChanged="OnSelectedIndexChanged" OnRowDataBound="OnRowDataBound"></asp:GridView>
    </div>
    <div style ="padding: 20px">
        <asp:GridView ID="gvReviews" runat="server"></asp:GridView>
    </div>
</asp:Content>
