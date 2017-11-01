<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Explore.aspx.cs" Inherits="HelloWorld.Explore.Explore" Async="true" AsyncTimeout="600000" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding: 20px; text-align: center">
        <h2>Where do you want to go?</h2>
        <span style="display: block; overflow: hidden; padding-right: 10px;">
            <asp:TextBox ID="txtExplore" runat="server" Width="1500px"></asp:TextBox>
            <asp:Button ID="btnExploreCustom" runat="server" Text="Go" OnClick="btnExploreCustom_Click" />
        </span>
        <span>
            <asp:Label ID="lblInfo" runat="server" Text="* Paste TripAdvisor url (e.g https://www.tripadvisor.com/Tourism-g1758900-Taguig_City_Metro_Manila_Luzon-Vacations.html)"></asp:Label>
        </span>
    </div>
    <div style="text-align: center">
        <%--<asp:Button ID="btnExplore" runat="server" Text="Explore Taguig" OnClick="btnExplore_Click" />--%>
    </div>
    <div style="padding: 20px">
        <h3>Check out one of these</h3>
        <asp:GridView ID="gvExplore" runat="server"
            OnSelectedIndexChanged="OnSelectedIndexChanged" OnRowDataBound="OnRowDataBound"
            ShowHeaderWhenEmpty="True" EmptyDataText="No records Found">
        </asp:GridView>
    </div>
    <div style="padding: 20px">
        <h3>What others are saying</h3>
        <asp:GridView ID="gvReviews" runat="server"
            ShowHeaderWhenEmpty="True" EmptyDataText="No records Found">
        </asp:GridView>
    </div>
    <div style="padding: 20px">
        <h3>Sights and scenes</h3>
        <div id="container" style="overflow: auto;">
            <table id="idSlider2" border="0" > 
                <tbody>
                    <tr>
                        <asp:Repeater ID="rpPhotos" runat="server">
                            <ItemTemplate>
                                <td class="td_f">
                                    <a href="<%# Eval("img-src") %>" target="_blank">
                                        <img src="<%# Eval("img-src") %>" width="250" height="250">
                                    </a>
                                &nbsp;</td>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</asp:Content>
