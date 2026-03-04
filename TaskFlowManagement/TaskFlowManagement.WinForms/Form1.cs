using TaskFlowManagement.Core.Entities;
using TaskFlowManagement.Core.Interfaces;

namespace TaskFlowManagement.WinForms
{
    public partial class Form1 : Form
    {
        private readonly ITaskRepository _taskRepository;
        private int _currentPage = 1;
        private const int PageSize = 30;

        public Form1(ITaskRepository taskRepository)
        {
            InitializeComponent();
            _taskRepository = taskRepository;

            this.Load += Form1_Load;
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            dgvTasks.CellClick += dgvTasks_CellClick;
            btnUpdate.Click += btnUpdate_Click;
            btnPrevious.Click += BtnPrevious_Click;
            btnNext.Click += BtnNext_Click;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadTasks();
        }

        private async Task LoadTasks()
        {
            var tasks = await _taskRepository.GetTasksPagedAsync(_currentPage, PageSize);
            dgvTasks.DataSource = tasks;
            lblPage.Text = $"Page {_currentPage}";
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Title không được để trống.");
                return;
            }

            var newTask = new TaskItem
            {
                Title = txtTitle.Text,
                CreatedAt = DateTime.Now
            };

            await _taskRepository.AddAsync(newTask);

            txtTitle.Clear();

            await LoadTasks(); // reload lại bảng
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTasks.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một task để xóa.");
                return;
            }

            var selectedTask = dgvTasks.CurrentRow.DataBoundItem as TaskItem;

            if (selectedTask == null)
                return;

            var confirm = MessageBox.Show(
                "Bạn có chắc muốn xóa task này?",
                "Confirm",
                MessageBoxButtons.YesNo);

            if (confirm != DialogResult.Yes)
                return;

            await _taskRepository.DeleteAsync(selectedTask.Id);

            await LoadTasks();
        }

        private int selectedTaskId = 0;

        private void dgvTasks_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvTasks.Rows[e.RowIndex];

                var selectedTask = row.DataBoundItem as TaskItem;
                if (selectedTask != null)
                {
                    selectedTaskId = selectedTask.Id;
                    txtTitle.Text = selectedTask.Title;
                }
            }
        }

        private async void btnUpdate_Click(object? sender, EventArgs e)
        {
            if (selectedTaskId == 0)
            {
                MessageBox.Show("Please select a task to update.");
                return;
            }

            var task = await _taskRepository.GetByIdAsync(selectedTaskId);

            if (task != null)
            {
                task.Title = txtTitle.Text;
                await _taskRepository.UpdateAsync(task);

                MessageBox.Show("Updated successfully!");

                selectedTaskId = 0;
                txtTitle.Clear();

                await LoadTasks();
            }
        }

        private async void BtnPrevious_Click(object? sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await LoadTasks();
            }
        }

        private async void BtnNext_Click(object? sender, EventArgs e)
        {
            _currentPage++;
            await LoadTasks();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
        }
    }
}