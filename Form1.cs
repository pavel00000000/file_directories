namespace file_directories
{
    public partial class Form1 : Form
    {
        private ImageList imageList;

        public Form1()
        {
            InitializeComponent();
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView1.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);

            SetupTreeView();
            PopulateDrives();
        }

        private void SetupTreeView()
        {
            imageList = new ImageList();
            treeView1.ImageList = imageList;
        }

        private void PopulateDrives()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                TreeNode driveNode = new TreeNode(drive.Name);
                driveNode.Tag = drive.Name; // Сохраняем полный путь в свойстве Tag для последующего использования
                treeView1.Nodes.Add(driveNode);
                driveNode.Nodes.Add(new TreeNode("...")); // Placeholder node для показа "+" рядом с узлом
            }
        }
        private void AddFiles(TreeNode node, string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            foreach (var file in directoryInfo.GetFiles())
            {
                TreeNode fileNode = new TreeNode(file.Name);
                // Если вы хотите сохранить путь к файлу или другую информацию:
                fileNode.Tag = file.FullName;
                node.Nodes.Add(fileNode);
            }
        }


        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode expandingNode = e.Node;
            expandingNode.Nodes.Clear(); // Очистка placeholder'ов

            string path = (string)expandingNode.Tag; // Получаем путь из свойства Tag

            try
            {
                var directoryInfo = new DirectoryInfo(path);

                // Добавляем подкаталоги
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    if (directory.Name == "$RECYCLE.BIN") continue; // Пропускаем папку $RECYCLE.BIN

                    TreeNode directoryNode = new TreeNode(directory.Name);
                    directoryNode.Tag = directory.FullName; // Сохраняем полный путь
                    expandingNode.Nodes.Add(directoryNode);

                    // Если в папке есть подпапки или файлы
                    if (directory.GetDirectories().Any() || directory.GetFiles().Any())
                    {
                        directoryNode.Nodes.Add(new TreeNode("...")); // Добавляем placeholder
                    }
                }

                // Добавляем файлы
                AddFiles(expandingNode, path);
            }
            catch (UnauthorizedAccessException) // Ловим ошибку доступа
            {
                // Можно вывести сообщение или просто пропустить текущий каталог
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении каталогов или файлов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode clickedNode = e.Node;

            // Проверка, является ли выбранный узел файлом
            if (clickedNode.Tag is string filePath && File.Exists(filePath))
            {
                try
                {
                    string fileExtension = Path.GetExtension(filePath).ToLower();
                    if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".bmp")
                    {
                        pictureBox1.Image = Image.FromFile(filePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
