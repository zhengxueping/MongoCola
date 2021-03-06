﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MongoDB.Bson;
using MongoUtility.Basic;
using ResourceLib.Utility;

namespace MongoGUICtl
{
    /// <summary>
    ///     BsonDoc 展示控件
    /// </summary>
    public partial class ctlTreeViewColumns : UserControl
    {
        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="GUIConfig">UIPackage</param>
        public ctlTreeViewColumns()
        {
            InitializeComponent();
            BackColor = VisualStyleInformation.TextControlBorder;
            Padding = new Padding(1);
        }

        [Description("TreeView associated with the control"), Category("Behavior")]
        public TreeView TreeView
        {
            get { return DatatreeView; }
        }

        [Description("Columns associated with the control"), Category("Behavior")]
        public ListView.ColumnHeaderCollection Columns
        {
            get { return listView.Columns; }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            DatatreeView.Focus();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_Click(object sender, EventArgs e)
        {
            var p = DatatreeView.PointToClient(MousePosition);
            var mTreeNode = DatatreeView.GetNodeAt(p);
            if (mTreeNode != null)
                DatatreeView.SelectedNode = mTreeNode;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            DatatreeView.Focus();
            DatatreeView.Invalidate();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            DatatreeView.Focus();
            DatatreeView.Invalidate();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = false;
            var rect = e.Bounds;
            if (rect.Height == 0)
            {
                //在展开节点的时候会出现根节点绘制错误的问题
                return;
            }
            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                e.Graphics.FillRectangle(
                    (e.State & TreeNodeStates.Focused) != 0 ? SystemBrushes.Highlight : SystemBrushes.Control, rect);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.White, rect);
            }
            var IndentWidth = DatatreeView.Indent*e.Node.Level + 25;
            e.Graphics.DrawRectangle(SystemPens.Control, rect);
            var StringRect = new Rectangle(e.Bounds.X + IndentWidth, e.Bounds.Y, colName.Width - IndentWidth,
                e.Bounds.Height);

            var TreeNameString = e.Node.Text;
            if (TreeNameString.EndsWith(ConstMgr.Array_Mark))
            {
                //Array_Mark 在计算路径的时候使用，不过，在表示的时候，则不能表示
                TreeNameString = TreeNameString.Substring(0, TreeNameString.Length - ConstMgr.Array_Mark.Length);
            }
            if (TreeNameString.EndsWith(ConstMgr.Document_Mark))
            {
                //Document_Mark 在计算路径的时候使用，不过，在表示的时候，则不能表示
                TreeNameString = TreeNameString.Substring(0, TreeNameString.Length - ConstMgr.Document_Mark.Length);
            }
            //感谢cyrus的建议，选中节点的文字表示，底色变更
            if ((e.State & TreeNodeStates.Selected) != 0 && (e.State & TreeNodeStates.Focused) != 0)
            {
                e.Graphics.DrawString(TreeNameString, Font, new SolidBrush(SystemColors.HighlightText), StringRect);
            }
            else
            {
                e.Graphics.DrawString(TreeNameString, Font, new SolidBrush(Color.Black), StringRect);
            }
            //CSHARP-1066: Change BsonElement from a class to a struct. 
            BsonElement mElement;
            if (e.Node.Tag != null)
            {
                if (e.Node.Tag.GetType() != typeof (BsonElement))
                {
                    mElement = new BsonElement("", (BsonValue) e.Node.Tag);
                }
                else
                {
                    mElement = (BsonElement) e.Node.Tag;
                }
            }
            var mValue = e.Node.Tag as BsonValue;
            //画框
            if (e.Node.GetNodeCount(true) > 0 ||
                (mElement.Value != null && (mElement.Value.IsBsonDocument || mElement.Value.IsBsonArray)))
            {
                //感谢Cyrus测试出来的问题：RenderWithVisualStyles应该加上去的。
                if (VisualStyleRenderer.IsSupported && Application.RenderWithVisualStyles)
                {
                    var LeftPoint = e.Bounds.X + IndentWidth - 20;
                    //感谢 Shadower http://home.cnblogs.com/u/14697/ 贡献的代码
                    var thisNode = e.Node;
                    var glyph = thisNode.IsExpanded
                        ? VisualStyleElement.TreeView.Glyph.Opened
                        : VisualStyleElement.TreeView.Glyph.Closed;
                    var vsr = new VisualStyleRenderer(glyph);
                    vsr.DrawBackground(e.Graphics, new Rectangle(LeftPoint, e.Bounds.Y + 4, 16, 16));
                }
                else
                {
                    var LeftPoint = e.Bounds.X + IndentWidth - 20;
                    e.Graphics.DrawRectangle(new Pen(Color.Black), new Rectangle(LeftPoint, e.Bounds.Y + 4, 12, 12));
                    var LeftMid = new Point(LeftPoint + 2, e.Bounds.Y + 10);
                    var RightMid = new Point(LeftPoint + 10, e.Bounds.Y + 10);
                    var TopMid = new Point(LeftPoint + 6, e.Bounds.Y + 6);
                    var BottomMid = new Point(LeftPoint + 6, e.Bounds.Y + 14);
                    e.Graphics.DrawLine(new Pen(Color.Black), LeftMid, RightMid);
                    if (!e.Node.IsExpanded)
                    {
                        e.Graphics.DrawLine(new Pen(Color.Black), TopMid, BottomMid);
                    }
                }
            }

            for (var intColumn = 1; intColumn < 3; intColumn++)
            {
                rect.Offset(listView.Columns[intColumn - 1].Width, 0);
                rect.Width = listView.Columns[intColumn].Width;
                e.Graphics.DrawRectangle(SystemPens.Control, rect);
                if (mElement.Value != null || mValue != null)
                {
                    var strColumnText = string.Empty;
                    if (intColumn == 1)
                    {
                        if (mElement.Value != null)
                        {
                            if (!mElement.Value.IsBsonDocument && !mElement.Value.IsBsonArray)
                            {
                                strColumnText = mElement.Value.ToString();
                            }
                        }
                        else
                        {
                            if (mValue != null)
                            {
                                //Type这里已经有表示Type的标识了，这里就不重复显示了。
                                if (!mValue.IsBsonDocument && !mValue.IsBsonArray)
                                {
                                    if (e.Node.Level > 0)
                                    {
                                        //根节点有Value，可能是ID，用来取得选中节点的信息
                                        strColumnText = mValue.ToString();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        strColumnText = mElement.Value != null
                            ? mElement.Value.GetType().Name.Substring(4)
                            : mValue.GetType().Name.Substring(4);
                    }

                    var flags = TextFormatFlags.EndEllipsis;
                    switch (listView.Columns[intColumn].TextAlign)
                    {
                        case HorizontalAlignment.Center:
                            flags |= TextFormatFlags.HorizontalCenter;
                            break;
                        case HorizontalAlignment.Left:
                            flags |= TextFormatFlags.Left;
                            break;
                        case HorizontalAlignment.Right:
                            flags |= TextFormatFlags.Right;
                            break;
                        default:
                            break;
                    }

                    rect.Y++;
                    if ((e.State & TreeNodeStates.Selected) != 0 &&
                        (e.State & TreeNodeStates.Focused) != 0)
                        TextRenderer.DrawText(e.Graphics, strColumnText, e.Node.NodeFont, rect,
                            SystemColors.HighlightText, flags);
                    else
                        TextRenderer.DrawText(e.Graphics, strColumnText, e.Node.NodeFont, rect, e.Node.ForeColor,
                            e.Node.BackColor, flags);
                    rect.Y--;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_SizeChanged(object sender, EventArgs e)
        {
            colName.Width = Convert.ToInt32(Width*0.3);
            colValue.Width = Convert.ToInt32(Width*0.45);
            colType.Width = Convert.ToInt32(Width*0.2);
        }

        private void ctlTreeViewColumnsLoad(object sender, EventArgs e)
        {
            if (configuration.guiConfig == null) return;
            if (!configuration.guiConfig.IsUseDefaultLanguage)
            {
                colName.Text = configuration.guiConfig.MStringResource.GetText(TextType.Common_Name);
                colValue.Text = configuration.guiConfig.MStringResource.GetText(TextType.Common_Value);
                colType.Text = configuration.guiConfig.MStringResource.GetText(TextType.Common_Type);
            }
        }
    }
}