﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proj1.HVE.WebParts.HVEGetSQL1 {
    using System.Web.UI.WebControls.Expressions;
    using System.Web.UI.HtmlControls;
    using System.Collections;
    using System.Text;
    using System.Web.UI;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.SharePoint.WebPartPages;
    using System.Web.SessionState;
    using System.Configuration;
    using Microsoft.SharePoint;
    using System.Web;
    using System.Web.DynamicData;
    using System.Web.Caching;
    using System.Web.Profile;
    using System.ComponentModel.DataAnnotations;
    using System.Web.UI.WebControls;
    using System.Web.Security;
    using System;
    using Microsoft.SharePoint.Utilities;
    using System.Text.RegularExpressions;
    using System.Collections.Specialized;
    using System.Web.UI.WebControls.WebParts;
    using Microsoft.SharePoint.WebControls;
    
    
    public partial class HVEGetSQL1 {
        
        protected global::System.Web.UI.WebControls.GridView GridView1;
        
        protected global::System.Web.UI.WebControls.Repeater Repeater1;
        
        protected global::System.Web.UI.WebControls.Button Button1;
        
        protected global::System.Web.UI.WebControls.Label Label1;
        
        protected global::System.Web.UI.WebControls.Label Label2;
        
        protected global::System.Web.UI.WebControls.Label Label3;
        
        protected global::System.Web.UI.WebControls.Label Label4;
        
        protected global::System.Web.UI.WebControls.Label Label5;
        
        protected global::System.Web.UI.WebControls.Label Label7;
        
        protected global::System.Web.UI.WebControls.Label Label6;
        
        protected global::System.Web.UI.WebControls.Label Label8;
        
        protected global::System.Web.UI.WebControls.Label Label9;
        
        protected global::System.Web.UI.WebControls.Label Label10;
        
        protected global::System.Web.UI.WebControls.Literal LiteralText;
        
        protected global::System.Web.UI.WebControls.Label Label11;
        
        public static implicit operator global::System.Web.UI.TemplateControl(HVEGetSQL1 target) 
        {
            return target == null ? null : target.TemplateControl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.GridView @__BuildControlGridView1() {
            global::System.Web.UI.WebControls.GridView @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.GridView();
            this.GridView1 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "GridView1";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private void @__BuildControl__control2(System.Web.UI.Control @__ctrl) {
            System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n        <div style=\"font-weight: bold;\">Top searches</div>        \r\n    "));
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.DataBoundLiteralControl @__BuildControl__control4() {
            global::System.Web.UI.DataBoundLiteralControl @__ctrl;
            @__ctrl = new global::System.Web.UI.DataBoundLiteralControl(4, 3);
            @__ctrl.TemplateControl = this;
            @__ctrl.SetStaticString(0, "  \r\n        \r\n        <a href=\'");
            @__ctrl.SetStaticString(1, "\' title=\'");
            @__ctrl.SetStaticString(2, "\'>");
            @__ctrl.SetStaticString(3, "</a>\r\n    ");
            @__ctrl.DataBinding += new System.EventHandler(this.@__DataBind__control4);
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        public void @__DataBind__control4(object sender, System.EventArgs e) {
            System.Web.UI.WebControls.RepeaterItem Container;
            System.Web.UI.DataBoundLiteralControl target;
            target = ((System.Web.UI.DataBoundLiteralControl)(sender));
            Container = ((System.Web.UI.WebControls.RepeaterItem)(target.BindingContainer));
            target.SetDataBoundString(0, global::System.Convert.ToString(Eval("queryString"), global::System.Globalization.CultureInfo.CurrentCulture));
            target.SetDataBoundString(1, global::System.Convert.ToString(Eval("queryString"), global::System.Globalization.CultureInfo.CurrentCulture));
            target.SetDataBoundString(2, global::System.Convert.ToString(Eval("queryString"), global::System.Globalization.CultureInfo.CurrentCulture));
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private void @__BuildControl__control3(System.Web.UI.Control @__ctrl) {
            global::System.Web.UI.DataBoundLiteralControl @__ctrl1;
            @__ctrl1 = this.@__BuildControl__control4();
            System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
            @__parser.AddParsedSubObject(@__ctrl1);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Repeater @__BuildControlRepeater1() {
            global::System.Web.UI.WebControls.Repeater @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Repeater();
            this.Repeater1 = @__ctrl;
            @__ctrl.HeaderTemplate = new System.Web.UI.CompiledTemplateBuilder(new System.Web.UI.BuildTemplateMethod(this.@__BuildControl__control2));
            @__ctrl.ItemTemplate = new System.Web.UI.CompiledTemplateBuilder(new System.Web.UI.BuildTemplateMethod(this.@__BuildControl__control3));
            @__ctrl.ID = "Repeater1";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Button @__BuildControlButton1() {
            global::System.Web.UI.WebControls.Button @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Button();
            this.Button1 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "Button1";
            @__ctrl.Text = "Button";
            @__ctrl.Click -= new System.EventHandler(this.Button1_Click);
            @__ctrl.Click += new System.EventHandler(this.Button1_Click);
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Label @__BuildControlLabel1() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.Label1 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "Label1";
            @__ctrl.Text = "Label";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Label @__BuildControlLabel2() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.Label2 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "Label2";
            @__ctrl.Text = "Label";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Label @__BuildControlLabel3() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.Label3 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "Label3";
            @__ctrl.Text = "Label";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Label @__BuildControlLabel4() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.Label4 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "Label4";
            @__ctrl.Text = "Label";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Label @__BuildControlLabel5() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.Label5 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "Label5";
            @__ctrl.Text = "Label";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Label @__BuildControlLabel7() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.Label7 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "Label7";
            @__ctrl.Text = "Label";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Label @__BuildControlLabel6() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.Label6 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "Label6";
            @__ctrl.Text = "Label";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Label @__BuildControlLabel8() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.Label8 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "Label8";
            @__ctrl.Text = "Label";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Label @__BuildControlLabel9() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.Label9 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "Label9";
            @__ctrl.Text = "Label";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Label @__BuildControlLabel10() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.Label10 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "Label10";
            @__ctrl.Text = "Label";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.DataBoundLiteralControl @__BuildControl__control5() {
            global::System.Web.UI.DataBoundLiteralControl @__ctrl;
            @__ctrl = new global::System.Web.UI.DataBoundLiteralControl(2, 1);
            @__ctrl.SetStaticString(0, "\r\n\r\n<div id =\"Table1\">\r\n    <ul>\r\n        <li id =\"Row1\">>");
            @__ctrl.SetStaticString(1, "</li>\r\n    </ul>\r\n</div>\r\n<div>\r\n");
            @__ctrl.DataBinding += new System.EventHandler(this.@__DataBind__control5);
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        public void @__DataBind__control5(object sender, System.EventArgs e) {
            System.Web.UI.Control Container;
            System.Web.UI.DataBoundLiteralControl target;
            target = ((System.Web.UI.DataBoundLiteralControl)(sender));
            Container = ((System.Web.UI.Control)(target.BindingContainer));
            target.SetDataBoundString(0, global::System.Convert.ToString(Eval("queryString"), global::System.Globalization.CultureInfo.CurrentCulture));
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Literal @__BuildControlLiteralText() {
            global::System.Web.UI.WebControls.Literal @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Literal();
            this.LiteralText = @__ctrl;
            @__ctrl.ID = "LiteralText";
            @__ctrl.Text = "This is example of Literal";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Label @__BuildControlLabel11() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.Label11 = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "Label11";
            @__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingLabel11);
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        public void @__DataBindingLabel11(object sender, System.EventArgs e) {
            System.Web.UI.WebControls.Label dataBindingExpressionBuilderTarget;
            System.Web.UI.Control Container;
            dataBindingExpressionBuilderTarget = ((System.Web.UI.WebControls.Label)(sender));
            Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
            dataBindingExpressionBuilderTarget.Text = global::System.Convert.ToString(Eval("linkVal"), global::System.Globalization.CultureInfo.CurrentCulture);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private void @__BuildControlTree(global::Proj1.HVE.WebParts.HVEGetSQL1.HVEGetSQL1 @__ctrl) {
            global::System.Web.UI.WebControls.GridView @__ctrl1;
            @__ctrl1 = this.@__BuildControlGridView1();
            System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
            @__parser.AddParsedSubObject(@__ctrl1);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n\r\n\r\n"));
            global::System.Web.UI.WebControls.Repeater @__ctrl2;
            @__ctrl2 = this.@__BuildControlRepeater1();
            @__parser.AddParsedSubObject(@__ctrl2);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n<br />\r\n"));
            global::System.Web.UI.WebControls.Button @__ctrl3;
            @__ctrl3 = this.@__BuildControlButton1();
            @__parser.AddParsedSubObject(@__ctrl3);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n<br />\r\n"));
            global::System.Web.UI.WebControls.Label @__ctrl4;
            @__ctrl4 = this.@__BuildControlLabel1();
            @__parser.AddParsedSubObject(@__ctrl4);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n<p>\r\n    &nbsp;</p>\r\n"));
            global::System.Web.UI.WebControls.Label @__ctrl5;
            @__ctrl5 = this.@__BuildControlLabel2();
            @__parser.AddParsedSubObject(@__ctrl5);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n\r\n"));
            global::System.Web.UI.WebControls.Label @__ctrl6;
            @__ctrl6 = this.@__BuildControlLabel3();
            @__parser.AddParsedSubObject(@__ctrl6);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
            global::System.Web.UI.WebControls.Label @__ctrl7;
            @__ctrl7 = this.@__BuildControlLabel4();
            @__parser.AddParsedSubObject(@__ctrl7);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
            global::System.Web.UI.WebControls.Label @__ctrl8;
            @__ctrl8 = this.@__BuildControlLabel5();
            @__parser.AddParsedSubObject(@__ctrl8);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
            global::System.Web.UI.WebControls.Label @__ctrl9;
            @__ctrl9 = this.@__BuildControlLabel7();
            @__parser.AddParsedSubObject(@__ctrl9);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
            global::System.Web.UI.WebControls.Label @__ctrl10;
            @__ctrl10 = this.@__BuildControlLabel6();
            @__parser.AddParsedSubObject(@__ctrl10);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
            global::System.Web.UI.WebControls.Label @__ctrl11;
            @__ctrl11 = this.@__BuildControlLabel8();
            @__parser.AddParsedSubObject(@__ctrl11);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
            global::System.Web.UI.WebControls.Label @__ctrl12;
            @__ctrl12 = this.@__BuildControlLabel9();
            @__parser.AddParsedSubObject(@__ctrl12);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
            global::System.Web.UI.WebControls.Label @__ctrl13;
            @__ctrl13 = this.@__BuildControlLabel10();
            @__parser.AddParsedSubObject(@__ctrl13);
            global::System.Web.UI.DataBoundLiteralControl @__ctrl14;
            @__ctrl14 = this.@__BuildControl__control5();
            @__parser.AddParsedSubObject(@__ctrl14);
            global::System.Web.UI.WebControls.Literal @__ctrl15;
            @__ctrl15 = this.@__BuildControlLiteralText();
            @__parser.AddParsedSubObject(@__ctrl15);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n    </div>\r\n<div>\r\n    "));
            global::System.Web.UI.WebControls.Label @__ctrl16;
            @__ctrl16 = this.@__BuildControlLabel11();
            @__parser.AddParsedSubObject(@__ctrl16);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n</div>\r\n\r\n"));
        }
        
        private void InitializeControl() {
            this.@__BuildControlTree(this);
            this.Load += new global::System.EventHandler(this.Page_Load);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual object Eval(string expression) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual string Eval(string expression, string format) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression, format);
        }
    }
}