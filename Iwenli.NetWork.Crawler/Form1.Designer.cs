namespace Iwenli.NetWork.Crawler
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pgPage = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.pgPicDownload = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.lblStatPicDownload = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.lblPgSt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pgPage
            // 
            this.pgPage.Location = new System.Drawing.Point(12, 28);
            this.pgPage.Name = "pgPage";
            this.pgPage.Size = new System.Drawing.Size(682, 23);
            this.pgPage.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "页面抓取进度";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pgPicDownload
            // 
            this.pgPicDownload.Location = new System.Drawing.Point(12, 78);
            this.pgPicDownload.Name = "pgPicDownload";
            this.pgPicDownload.Size = new System.Drawing.Size(682, 23);
            this.pgPicDownload.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "图片下载进度";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatPicDownload
            // 
            this.lblStatPicDownload.Location = new System.Drawing.Point(240, 60);
            this.lblStatPicDownload.Name = "lblStatPicDownload";
            this.lblStatPicDownload.Size = new System.Drawing.Size(454, 16);
            this.lblStatPicDownload.TabIndex = 1;
            this.lblStatPicDownload.Text = "－";
            this.lblStatPicDownload.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLog
            // 
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(12, 119);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(680, 517);
            this.txtLog.TabIndex = 2;
            // 
            // btnGo
            // 
            this.btnGo.Font = new System.Drawing.Font("Consolas", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGo.Location = new System.Drawing.Point(240, 642);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(222, 42);
            this.btnGo.TabIndex = 3;
            this.btnGo.Text = "GO";
            this.btnGo.UseVisualStyleBackColor = true;
            // 
            // lblPgSt
            // 
            this.lblPgSt.Location = new System.Drawing.Point(292, 9);
            this.lblPgSt.Name = "lblPgSt";
            this.lblPgSt.Size = new System.Drawing.Size(402, 16);
            this.lblPgSt.TabIndex = 1;
            this.lblPgSt.Text = "－";
            this.lblPgSt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 700);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblStatPicDownload);
            this.Controls.Add(this.lblPgSt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pgPicDownload);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pgPage);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "抓取美女图片";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pgPage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar pgPicDownload;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblStatPicDownload;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Label lblPgSt;
    }
}

