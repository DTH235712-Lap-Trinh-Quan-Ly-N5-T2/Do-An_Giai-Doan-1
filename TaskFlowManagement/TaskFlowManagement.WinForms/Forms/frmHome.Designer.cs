namespace TaskFlowManagement.WinForms.Forms
{
    partial class frmHome
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelHeader     = new System.Windows.Forms.Panel();
            this.lblGreeting     = new System.Windows.Forms.Label();
            this.lblRole         = new System.Windows.Forms.Label();
            this.lblLastLogin    = new System.Windows.Forms.Label();
            this.panelAccentLine = new System.Windows.Forms.Panel();
            this.panelStats      = new System.Windows.Forms.Panel();
            this.panelCard1      = new System.Windows.Forms.Panel();
            this.lblCard1Icon    = new System.Windows.Forms.Label();
            this.lblCard1Title   = new System.Windows.Forms.Label();
            this.lblStatProjects = new System.Windows.Forms.Label();
            this.panelCard2      = new System.Windows.Forms.Panel();
            this.lblCard2Icon    = new System.Windows.Forms.Label();
            this.lblCard2Title   = new System.Windows.Forms.Label();
            this.lblStatTasks    = new System.Windows.Forms.Label();
            this.panelCard3      = new System.Windows.Forms.Panel();
            this.lblCard3Icon    = new System.Windows.Forms.Label();
            this.lblCard3Title   = new System.Windows.Forms.Label();
            this.lblStatOverdue  = new System.Windows.Forms.Label();
            this.panelCard4      = new System.Windows.Forms.Panel();
            this.lblCard4Icon    = new System.Windows.Forms.Label();
            this.lblCard4Title   = new System.Windows.Forms.Label();
            this.lblStatDone     = new System.Windows.Forms.Label();
            this.lblNote         = new System.Windows.Forms.Label();

            this.panelHeader.SuspendLayout();
            this.panelStats.SuspendLayout();
            this.SuspendLayout();

            // panelHeader
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.panelHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height    = 110;
            this.panelHeader.Name      = "panelHeader";
            this.panelHeader.Controls.AddRange(new System.Windows.Forms.Control[]
            { this.lblGreeting, this.lblRole, this.lblLastLogin, this.panelAccentLine });

            this.panelAccentLine.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.panelAccentLine.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.panelAccentLine.Height    = 3;
            this.panelAccentLine.Name      = "panelAccentLine";

            this.lblGreeting.AutoSize  = false;
            this.lblGreeting.Font      = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblGreeting.ForeColor = System.Drawing.Color.White;
            this.lblGreeting.Location  = new System.Drawing.Point(24, 14);
            this.lblGreeting.Name      = "lblGreeting";
            this.lblGreeting.Size      = new System.Drawing.Size(900, 38);
            this.lblGreeting.Text      = "Chào buổi sáng, ...! 👋";

            this.lblRole.AutoSize  = false;
            this.lblRole.Font      = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblRole.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblRole.Location  = new System.Drawing.Point(26, 54);
            this.lblRole.Name      = "lblRole";
            this.lblRole.Size      = new System.Drawing.Size(400, 22);
            this.lblRole.Text      = "Vai trò: ...";

            this.lblLastLogin.AutoSize  = false;
            this.lblLastLogin.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLastLogin.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblLastLogin.Location  = new System.Drawing.Point(26, 78);
            this.lblLastLogin.Name      = "lblLastLogin";
            this.lblLastLogin.Size      = new System.Drawing.Size(500, 20);
            this.lblLastLogin.Text      = "";

            // panelStats
            this.panelStats.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.panelStats.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.panelStats.Name      = "panelStats";
            this.panelStats.Controls.AddRange(new System.Windows.Forms.Control[]
            { this.panelCard1, this.panelCard2, this.panelCard3, this.panelCard4, this.lblNote });

            // Card 1
            this.panelCard1.BackColor   = System.Drawing.Color.White;
            this.panelCard1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCard1.Location    = new System.Drawing.Point(24, 24);
            this.panelCard1.Name        = "panelCard1";
            this.panelCard1.Size        = new System.Drawing.Size(200, 120);
            this.panelCard1.Controls.AddRange(new System.Windows.Forms.Control[]
            { this.lblCard1Icon, this.lblCard1Title, this.lblStatProjects });

            this.lblCard1Icon.Font = new System.Drawing.Font("Segoe UI Emoji", 26F);
            this.lblCard1Icon.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lblCard1Icon.Location = new System.Drawing.Point(16, 10);
            this.lblCard1Icon.Name = "lblCard1Icon";
            this.lblCard1Icon.Size = new System.Drawing.Size(50, 46);
            this.lblCard1Icon.Text = "📁";
            this.lblCard1Icon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblCard1Title.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblCard1Title.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblCard1Title.Location = new System.Drawing.Point(16, 60);
            this.lblCard1Title.Name = "lblCard1Title";
            this.lblCard1Title.Size = new System.Drawing.Size(170, 18);
            this.lblCard1Title.Text = "DỰ ÁN ĐANG CHẠY";

            this.lblStatProjects.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblStatProjects.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lblStatProjects.Location = new System.Drawing.Point(16, 78);
            this.lblStatProjects.Name = "lblStatProjects";
            this.lblStatProjects.Size = new System.Drawing.Size(170, 36);
            this.lblStatProjects.Text = "—";

            // Card 2
            this.panelCard2.BackColor   = System.Drawing.Color.White;
            this.panelCard2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCard2.Location    = new System.Drawing.Point(244, 24);
            this.panelCard2.Name        = "panelCard2";
            this.panelCard2.Size        = new System.Drawing.Size(200, 120);
            this.panelCard2.Controls.AddRange(new System.Windows.Forms.Control[]
            { this.lblCard2Icon, this.lblCard2Title, this.lblStatTasks });

            this.lblCard2Icon.Font = new System.Drawing.Font("Segoe UI Emoji", 26F);
            this.lblCard2Icon.ForeColor = System.Drawing.Color.FromArgb(5, 150, 105);
            this.lblCard2Icon.Location = new System.Drawing.Point(16, 10);
            this.lblCard2Icon.Name = "lblCard2Icon";
            this.lblCard2Icon.Size = new System.Drawing.Size(50, 46);
            this.lblCard2Icon.Text = "✅";
            this.lblCard2Icon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblCard2Title.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblCard2Title.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblCard2Title.Location = new System.Drawing.Point(16, 60);
            this.lblCard2Title.Name = "lblCard2Title";
            this.lblCard2Title.Size = new System.Drawing.Size(170, 18);
            this.lblCard2Title.Text = "CÔNG VIỆC CỦA TÔI";

            this.lblStatTasks.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblStatTasks.ForeColor = System.Drawing.Color.FromArgb(5, 150, 105);
            this.lblStatTasks.Location = new System.Drawing.Point(16, 78);
            this.lblStatTasks.Name = "lblStatTasks";
            this.lblStatTasks.Size = new System.Drawing.Size(170, 36);
            this.lblStatTasks.Text = "—";

            // Card 3
            this.panelCard3.BackColor   = System.Drawing.Color.White;
            this.panelCard3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCard3.Location    = new System.Drawing.Point(464, 24);
            this.panelCard3.Name        = "panelCard3";
            this.panelCard3.Size        = new System.Drawing.Size(200, 120);
            this.panelCard3.Controls.AddRange(new System.Windows.Forms.Control[]
            { this.lblCard3Icon, this.lblCard3Title, this.lblStatOverdue });

            this.lblCard3Icon.Font = new System.Drawing.Font("Segoe UI Emoji", 26F);
            this.lblCard3Icon.ForeColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.lblCard3Icon.Location = new System.Drawing.Point(16, 10);
            this.lblCard3Icon.Name = "lblCard3Icon";
            this.lblCard3Icon.Size = new System.Drawing.Size(50, 46);
            this.lblCard3Icon.Text = "⚠";
            this.lblCard3Icon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblCard3Title.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblCard3Title.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblCard3Title.Location = new System.Drawing.Point(16, 60);
            this.lblCard3Title.Name = "lblCard3Title";
            this.lblCard3Title.Size = new System.Drawing.Size(170, 18);
            this.lblCard3Title.Text = "QUÁ HẠN";

            this.lblStatOverdue.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblStatOverdue.ForeColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.lblStatOverdue.Location = new System.Drawing.Point(16, 78);
            this.lblStatOverdue.Name = "lblStatOverdue";
            this.lblStatOverdue.Size = new System.Drawing.Size(170, 36);
            this.lblStatOverdue.Text = "—";

            // Card 4
            this.panelCard4.BackColor   = System.Drawing.Color.White;
            this.panelCard4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCard4.Location    = new System.Drawing.Point(684, 24);
            this.panelCard4.Name        = "panelCard4";
            this.panelCard4.Size        = new System.Drawing.Size(200, 120);
            this.panelCard4.Controls.AddRange(new System.Windows.Forms.Control[]
            { this.lblCard4Icon, this.lblCard4Title, this.lblStatDone });

            this.lblCard4Icon.Font = new System.Drawing.Font("Segoe UI Emoji", 26F);
            this.lblCard4Icon.ForeColor = System.Drawing.Color.FromArgb(124, 58, 237);
            this.lblCard4Icon.Location = new System.Drawing.Point(16, 10);
            this.lblCard4Icon.Name = "lblCard4Icon";
            this.lblCard4Icon.Size = new System.Drawing.Size(50, 46);
            this.lblCard4Icon.Text = "🎯";
            this.lblCard4Icon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblCard4Title.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblCard4Title.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblCard4Title.Location = new System.Drawing.Point(16, 60);
            this.lblCard4Title.Name = "lblCard4Title";
            this.lblCard4Title.Size = new System.Drawing.Size(170, 18);
            this.lblCard4Title.Text = "HOÀN THÀNH THÁNG NÀY";

            this.lblStatDone.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblStatDone.ForeColor = System.Drawing.Color.FromArgb(124, 58, 237);
            this.lblStatDone.Location = new System.Drawing.Point(16, 78);
            this.lblStatDone.Name = "lblStatDone";
            this.lblStatDone.Size = new System.Drawing.Size(170, 36);
            this.lblStatDone.Text = "—";

            // lblNote
            this.lblNote.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNote.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblNote.Location  = new System.Drawing.Point(24, 160);
            this.lblNote.Name      = "lblNote";
            this.lblNote.Size      = new System.Drawing.Size(860, 22);
            this.lblNote.Text      = "ℹ️  Số liệu thống kê sẽ được cập nhật từ Giai đoạn 2 trở đi.";

            // frmHome
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(241, 245, 249);
            this.ClientSize          = new System.Drawing.Size(960, 540);
            this.Controls.Add(this.panelStats);
            this.Controls.Add(this.panelHeader);
            this.Font          = new System.Drawing.Font("Segoe UI", 9.5F);
            this.Name          = "frmHome";
            this.Text          = "Trang chủ";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;

            this.panelHeader.ResumeLayout(false);
            this.panelStats.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        // ── Field declarations ───────────────────────────────────
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblGreeting;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Label lblLastLogin;
        private System.Windows.Forms.Panel panelAccentLine;
        private System.Windows.Forms.Panel panelStats;
        private System.Windows.Forms.Panel panelCard1;
        private System.Windows.Forms.Label lblCard1Icon;
        private System.Windows.Forms.Label lblCard1Title;
        private System.Windows.Forms.Label lblStatProjects;
        private System.Windows.Forms.Panel panelCard2;
        private System.Windows.Forms.Label lblCard2Icon;
        private System.Windows.Forms.Label lblCard2Title;
        private System.Windows.Forms.Label lblStatTasks;
        private System.Windows.Forms.Panel panelCard3;
        private System.Windows.Forms.Label lblCard3Icon;
        private System.Windows.Forms.Label lblCard3Title;
        private System.Windows.Forms.Label lblStatOverdue;
        private System.Windows.Forms.Panel panelCard4;
        private System.Windows.Forms.Label lblCard4Icon;
        private System.Windows.Forms.Label lblCard4Title;
        private System.Windows.Forms.Label lblStatDone;
        private System.Windows.Forms.Label lblNote;
    }
}
