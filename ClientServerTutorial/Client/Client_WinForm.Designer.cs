namespace CNA_Client {
    partial class Client_WinForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.MessageWindow = new System.Windows.Forms.TextBox();
            this.InputField = new System.Windows.Forms.TextBox();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.NickName_label = new System.Windows.Forms.Label();
            this.ChangeName_Button = new System.Windows.Forms.Button();
            this.ChangeName_textbox = new System.Windows.Forms.TextBox();
            this.UserList = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Clear_Selected = new System.Windows.Forms.Button();
            this.LaunchGame = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MessageWindow
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.MessageWindow, 2);
            this.MessageWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessageWindow.Location = new System.Drawing.Point(273, 10);
            this.MessageWindow.Margin = new System.Windows.Forms.Padding(10);
            this.MessageWindow.Multiline = true;
            this.MessageWindow.Name = "MessageWindow";
            this.MessageWindow.ReadOnly = true;
            this.tableLayoutPanel1.SetRowSpan(this.MessageWindow, 4);
            this.MessageWindow.Size = new System.Drawing.Size(501, 353);
            this.MessageWindow.TabIndex = 0;
            this.MessageWindow.Text = "Received Messages";
            // 
            // InputField
            // 
            this.InputField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.InputField.Location = new System.Drawing.Point(273, 381);
            this.InputField.Margin = new System.Windows.Forms.Padding(10, 0, 10, 10);
            this.InputField.MinimumSize = new System.Drawing.Size(400, 20);
            this.InputField.Name = "InputField";
            this.InputField.Size = new System.Drawing.Size(400, 20);
            this.InputField.TabIndex = 1;
            // 
            // SubmitButton
            // 
            this.SubmitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SubmitButton.AutoSize = true;
            this.SubmitButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SubmitButton.Location = new System.Drawing.Point(683, 373);
            this.SubmitButton.Margin = new System.Windows.Forms.Padding(0, 0, 10, 10);
            this.SubmitButton.MinimumSize = new System.Drawing.Size(90, 20);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(91, 28);
            this.SubmitButton.TabIndex = 2;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // NickName_label
            // 
            this.NickName_label.AutoSize = true;
            this.NickName_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.NickName_label, 2);
            this.NickName_label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NickName_label.Location = new System.Drawing.Point(10, 373);
            this.NickName_label.Margin = new System.Windows.Forms.Padding(10, 0, 10, 10);
            this.NickName_label.Name = "NickName_label";
            this.NickName_label.Size = new System.Drawing.Size(243, 28);
            this.NickName_label.TabIndex = 9;
            this.NickName_label.Text = "nickname";
            this.NickName_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ChangeName_Button
            // 
            this.ChangeName_Button.AutoSize = true;
            this.ChangeName_Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChangeName_Button.Location = new System.Drawing.Point(182, 339);
            this.ChangeName_Button.Margin = new System.Windows.Forms.Padding(10, 0, 10, 10);
            this.ChangeName_Button.Name = "ChangeName_Button";
            this.ChangeName_Button.Size = new System.Drawing.Size(71, 24);
            this.ChangeName_Button.TabIndex = 8;
            this.ChangeName_Button.Text = "Change Nick";
            this.ChangeName_Button.UseVisualStyleBackColor = true;
            this.ChangeName_Button.Click += new System.EventHandler(this.ChangeName_Button_Click);
            // 
            // ChangeName_textbox
            // 
            this.ChangeName_textbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChangeName_textbox.Location = new System.Drawing.Point(10, 339);
            this.ChangeName_textbox.Margin = new System.Windows.Forms.Padding(10, 0, 10, 10);
            this.ChangeName_textbox.Name = "ChangeName_textbox";
            this.ChangeName_textbox.Size = new System.Drawing.Size(152, 20);
            this.ChangeName_textbox.TabIndex = 7;
            this.ChangeName_textbox.Text = "Change name TextBox";
            this.ChangeName_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // UserList
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.UserList, 2);
            this.UserList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UserList.FormattingEnabled = true;
            this.UserList.Location = new System.Drawing.Point(10, 10);
            this.UserList.Margin = new System.Windows.Forms.Padding(10);
            this.UserList.Name = "UserList";
            this.UserList.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.UserList.Size = new System.Drawing.Size(243, 251);
            this.UserList.Sorted = true;
            this.UserList.TabIndex = 4;
            this.UserList.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.26709F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.26785F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.46506F));
            this.tableLayoutPanel1.Controls.Add(this.SubmitButton, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.MessageWindow, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.UserList, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ChangeName_Button, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.ChangeName_textbox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.InputField, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.NickName_label, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.Clear_Selected, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.LaunchGame, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65.95744F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.510638F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.510638F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.510638F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.510638F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 411);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // Clear_Selected
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.Clear_Selected, 2);
            this.Clear_Selected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Clear_Selected.Location = new System.Drawing.Point(10, 271);
            this.Clear_Selected.Margin = new System.Windows.Forms.Padding(10, 0, 10, 10);
            this.Clear_Selected.Name = "Clear_Selected";
            this.Clear_Selected.Size = new System.Drawing.Size(243, 24);
            this.Clear_Selected.TabIndex = 10;
            this.Clear_Selected.Text = "Clear Selected Users";
            this.Clear_Selected.UseVisualStyleBackColor = true;
            this.Clear_Selected.Click += new System.EventHandler(this.Clear_Selected_Click);
            // 
            // LaunchGame
            // 
            this.LaunchGame.AutoSize = true;
            this.LaunchGame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.LaunchGame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.tableLayoutPanel1.SetColumnSpan(this.LaunchGame, 2);
            this.LaunchGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LaunchGame.Location = new System.Drawing.Point(10, 305);
            this.LaunchGame.Margin = new System.Windows.Forms.Padding(10, 0, 10, 10);
            this.LaunchGame.MinimumSize = new System.Drawing.Size(100, 20);
            this.LaunchGame.Name = "LaunchGame";
            this.LaunchGame.Size = new System.Drawing.Size(243, 24);
            this.LaunchGame.TabIndex = 11;
            this.LaunchGame.Text = "Join Game";
            this.LaunchGame.UseVisualStyleBackColor = false;
            this.LaunchGame.Click += new System.EventHandler(this.LaunchGame_Click);
            // 
            // Client_WinForm
            // 
            this.AcceptButton = this.SubmitButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(784, 411);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 450);
            this.Name = "Client_WinForm";
            this.Text = "Client_WinForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Client_WinForm_FormClosing);
            this.Shown += new System.EventHandler(this.Client_WinForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox MessageWindow;
        private System.Windows.Forms.TextBox InputField;
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.Button ChangeName_Button;
        private System.Windows.Forms.TextBox ChangeName_textbox;
        private System.Windows.Forms.Label NickName_label;
        private System.Windows.Forms.ListBox UserList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button Clear_Selected;
        private System.Windows.Forms.Button LaunchGame;
    }
}