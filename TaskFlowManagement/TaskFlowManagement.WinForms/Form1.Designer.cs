namespace TaskFlowManagement.WinForms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private DataGridView dgvTasks;
        private TextBox txtTitle;
        private Button btnAdd;
        private Label lblTitle;
        private Button btnDelete;
        private Button btnUpdate;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private GroupBox grpTask;
        private Button btnPrevious;
        private Button btnNext;
        private Label lblPage;

        private void InitializeComponent()
        {
            this.dgvTasks = new DataGridView();
            this.txtTitle = new TextBox();
            this.btnAdd = new Button();
            this.btnDelete = new Button();
            this.btnUpdate = new Button();
            this.lblTitle = new Label();
            this.grpTask = new GroupBox();
            this.btnPrevious = new Button();
            this.btnNext = new Button();
            this.lblPage = new Label();

            ((System.ComponentModel.ISupportInitialize)(this.dgvTasks)).BeginInit();
            this.SuspendLayout();

            // ========================
            // FORM
            // ========================
            this.ClientSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "TaskFlow Management";

            // ========================
            // GROUPBOX
            // ========================
            this.grpTask.Text = "Task Information";
            this.grpTask.Location = new Point(20, 20);
            this.grpTask.Size = new Size(850, 100);

            // ========================
            // LABEL TITLE
            // ========================
            this.lblTitle.Text = "Title";
            this.lblTitle.Location = new Point(20, 40);
            this.lblTitle.AutoSize = true;

            // ========================
            // TEXTBOX TITLE
            // ========================
            this.txtTitle.Location = new Point(80, 37);
            this.txtTitle.Size = new Size(350, 23);

            // ========================
            // BUTTON ADD
            // ========================
            this.btnAdd.Text = "Add";
            this.btnAdd.Location = new Point(460, 35);
            this.btnAdd.Size = new Size(100, 30);

            // ========================
            // BUTTON UPDATE
            // ========================
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Location = new Point(580, 35);
            this.btnUpdate.Size = new Size(100, 30);

            // ========================
            // BUTTON DELETE
            // ========================
            this.btnDelete.Text = "Delete";
            this.btnDelete.Location = new Point(700, 35);
            this.btnDelete.Size = new Size(100, 30);

            // Add controls vào groupbox
            this.grpTask.Controls.Add(this.lblTitle);
            this.grpTask.Controls.Add(this.txtTitle);
            this.grpTask.Controls.Add(this.btnAdd);
            this.grpTask.Controls.Add(this.btnUpdate);
            this.grpTask.Controls.Add(this.btnDelete);

            // ========================
            // DATAGRIDVIEW
            // ========================
            this.dgvTasks.Location = new Point(20, 140);
            this.dgvTasks.Size = new Size(850, 350);
            this.dgvTasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTasks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvTasks.ReadOnly = true;
            this.dgvTasks.MultiSelect = false;
            this.dgvTasks.RowHeadersVisible = false;
            this.dgvTasks.AllowUserToAddRows = false;
            this.dgvTasks.BackgroundColor = Color.White;
            this.dgvTasks.BorderStyle = BorderStyle.FixedSingle;

            // ========================
            // PAGINATION
            // ========================
            this.btnPrevious.Text = "Previous";
            this.btnPrevious.Location = new Point(300, 510);
            this.btnPrevious.Size = new Size(100, 30);

            this.lblPage.Text = "Page 1";
            this.lblPage.Location = new Point(430, 515);
            this.lblPage.AutoSize = true;

            this.btnNext.Text = "Next";
            this.btnNext.Location = new Point(480, 510);
            this.btnNext.Size = new Size(100, 30);

            // ========================
            // ADD CONTROLS TO FORM
            // ========================
            this.Controls.Add(this.grpTask);
            this.Controls.Add(this.dgvTasks);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.lblPage);
            this.Controls.Add(this.btnNext);

            ((System.ComponentModel.ISupportInitialize)(this.dgvTasks)).EndInit();
            this.ResumeLayout(false);
        }
    }
}