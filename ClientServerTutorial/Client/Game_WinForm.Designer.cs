namespace CNA_Client {
    partial class Game_WinForm {

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
            this.ExitButton = new System.Windows.Forms.Button();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.MessageWindow = new System.Windows.Forms.TextBox();
            this.InputField = new System.Windows.Forms.TextBox();
            this.GameUpdateTextbox = new System.Windows.Forms.TextBox();
            this.gameControler1 = new InvadersGame_WinFormControl.GameControler();
            this.SuspendLayout();
            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(12, 415);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 23);
            this.ExitButton.TabIndex = 1;
            this.ExitButton.Text = "EXIT";
            this.ExitButton.UseVisualStyleBackColor = true;
            // 
            // SubmitButton
            // 
            this.SubmitButton.Location = new System.Drawing.Point(93, 415);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(75, 23);
            this.SubmitButton.TabIndex = 2;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Click += new System.EventHandler(this.ShowPlayerPos);
            // 
            // MessageWindow
            // 
            this.MessageWindow.Location = new System.Drawing.Point(12, 12);
            this.MessageWindow.Multiline = true;
            this.MessageWindow.Name = "MessageWindow";
            this.MessageWindow.Size = new System.Drawing.Size(156, 274);
            this.MessageWindow.TabIndex = 3;
            this.MessageWindow.Text = "MessageWindow";
            // 
            // InputField
            // 
            this.InputField.Location = new System.Drawing.Point(12, 364);
            this.InputField.Multiline = true;
            this.InputField.Name = "InputField";
            this.InputField.Size = new System.Drawing.Size(156, 45);
            this.InputField.TabIndex = 4;
            this.InputField.Text = "InputField";
            // 
            // GameUpdateTextbox
            // 
            this.GameUpdateTextbox.Location = new System.Drawing.Point(12, 292);
            this.GameUpdateTextbox.Multiline = true;
            this.GameUpdateTextbox.Name = "GameUpdateTextbox";
            this.GameUpdateTextbox.Size = new System.Drawing.Size(156, 66);
            this.GameUpdateTextbox.TabIndex = 6;
            this.GameUpdateTextbox.Text = "textBox1";
            // 
            // gameControler1
            // 
            this.gameControler1._ticks = 0F;
            this.gameControler1.Location = new System.Drawing.Point(174, 12);
            this.gameControler1.MouseHoverUpdatesOnly = false;
            this.gameControler1.Name = "gameControler1";
            this.gameControler1.Size = new System.Drawing.Size(614, 426);
            this.gameControler1.TabIndex = 7;
            this.gameControler1.Text = "gameControler1";
            this.gameControler1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.On_KeyDown);
            // 
            // Game_WinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gameControler1);
            this.Controls.Add(this.GameUpdateTextbox);
            this.Controls.Add(this.InputField);
            this.Controls.Add(this.MessageWindow);
            this.Controls.Add(this.SubmitButton);
            this.Controls.Add(this.ExitButton);
            this.Name = "Game_WinForm";
            this.Text = "Game_WinForm";
            this.Activated += new System.EventHandler(this.Game_WinForm_Activated);
            this.Deactivate += new System.EventHandler(this.Game_WinForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Game_WinForm_FormClosing);
            this.Load += new System.EventHandler(this.Game_WinForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.TextBox MessageWindow;
        private System.Windows.Forms.TextBox InputField;
        private System.Windows.Forms.TextBox GameUpdateTextbox;
        private InvadersGame_WinFormControl.GameControler gameControler1;
    }
}