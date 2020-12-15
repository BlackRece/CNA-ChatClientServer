namespace InvadersGame_WinFormControl
{
    partial class InvadersGameControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // InvadersGameControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "InvadersGameControl";
            this.Size = new System.Drawing.Size(800, 450);
            this.Enter += new System.EventHandler(this.InvadersGameControl_Enter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.On_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
