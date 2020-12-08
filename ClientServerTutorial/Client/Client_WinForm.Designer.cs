namespace Client {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.NickName_label = new System.Windows.Forms.Label();
            this.ChangeName_Button = new System.Windows.Forms.Button();
            this.ChangeName_textbox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MessageWindow
            // 
            this.MessageWindow.Location = new System.Drawing.Point(12, 12);
            this.MessageWindow.Multiline = true;
            this.MessageWindow.Name = "MessageWindow";
            this.MessageWindow.ReadOnly = true;
            this.MessageWindow.Size = new System.Drawing.Size(776, 358);
            this.MessageWindow.TabIndex = 0;
            // 
            // InputField
            // 
            this.InputField.Location = new System.Drawing.Point(12, 418);
            this.InputField.Name = "InputField";
            this.InputField.Size = new System.Drawing.Size(695, 20);
            this.InputField.TabIndex = 1;
            // 
            // SubmitButton
            // 
            this.SubmitButton.Location = new System.Drawing.Point(713, 415);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(75, 23);
            this.SubmitButton.TabIndex = 2;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.NickName_label);
            this.groupBox1.Controls.Add(this.ChangeName_Button);
            this.groupBox1.Controls.Add(this.ChangeName_textbox);
            this.groupBox1.Location = new System.Drawing.Point(12, 376);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(363, 36);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // NickName_label
            // 
            this.NickName_label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NickName_label.AutoSize = true;
            this.NickName_label.Location = new System.Drawing.Point(70, 12);
            this.NickName_label.Name = "NickName_label";
            this.NickName_label.Size = new System.Drawing.Size(53, 13);
            this.NickName_label.TabIndex = 9;
            this.NickName_label.Text = "nickname";
            // 
            // ChangeName_Button
            // 
            this.ChangeName_Button.Location = new System.Drawing.Point(282, 7);
            this.ChangeName_Button.Name = "ChangeName_Button";
            this.ChangeName_Button.Size = new System.Drawing.Size(75, 23);
            this.ChangeName_Button.TabIndex = 8;
            this.ChangeName_Button.Text = "Change Nick";
            this.ChangeName_Button.UseVisualStyleBackColor = true;
            this.ChangeName_Button.Click += new System.EventHandler(this.ChangeName_Button_Click);
            // 
            // ChangeName_textbox
            // 
            this.ChangeName_textbox.Location = new System.Drawing.Point(129, 7);
            this.ChangeName_textbox.Name = "ChangeName_textbox";
            this.ChangeName_textbox.Size = new System.Drawing.Size(147, 20);
            this.ChangeName_textbox.TabIndex = 7;
            // 
            // Client_WinForm
            // 
            this.AcceptButton = this.SubmitButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SubmitButton);
            this.Controls.Add(this.InputField);
            this.Controls.Add(this.MessageWindow);
            this.Name = "Client_WinForm";
            this.Text = "Client_WinForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Client_WinForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox MessageWindow;
        private System.Windows.Forms.TextBox InputField;
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ChangeName_Button;
        private System.Windows.Forms.TextBox ChangeName_textbox;
        private System.Windows.Forms.Label NickName_label;
    }
}