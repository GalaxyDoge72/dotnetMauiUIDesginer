namespace dotnetMauiUIDesginer
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            saveButton = new Button();
            mouseLocationLabel = new Label();
            oneMSTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // saveButton
            // 
            saveButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            saveButton.Location = new Point(1383, 9);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(79, 30);
            saveButton.TabIndex = 0;
            saveButton.Text = "Save...";
            saveButton.UseVisualStyleBackColor = true;
            // 
            // mouseLocationLabel
            // 
            mouseLocationLabel.AutoSize = true;
            mouseLocationLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            mouseLocationLabel.Location = new Point(1230, 14);
            mouseLocationLabel.Name = "mouseLocationLabel";
            mouseLocationLabel.Size = new Size(50, 20);
            mouseLocationLabel.TabIndex = 1;
            mouseLocationLabel.Text = "label1";
            // 
            // oneMSTimer
            // 
            oneMSTimer.Enabled = true;
            oneMSTimer.Interval = 1;
            oneMSTimer.Tick += oneMSTimer_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1474, 724);
            Controls.Add(mouseLocationLabel);
            Controls.Add(saveButton);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button saveButton;
        private Label mouseLocationLabel;
        private System.Windows.Forms.Timer oneMSTimer;
    }
}
