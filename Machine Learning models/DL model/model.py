import torch
import torch.nn as nn
import torch.optim as optim
from sklearn.model_selection import train_test_split
from torch.utils.data import TensorDataset, DataLoader
from sklearn.metrics import accuracy_score
import numpy as np

from readData import readData


# 定义二分类神经网络
class BinaryClassificationModel(nn.Module):
    def __init__(self, input_size):
        super(BinaryClassificationModel, self).__init__()
        self.fc1 = nn.Linear(input_size, 32)
        self.relu = nn.ReLU()
        self.fc2 = nn.Linear(32, 16)
        self.fc3 = nn.Linear(16, 1)
        self.sigmoid = nn.Sigmoid()

    def forward(self, x):
        x = self.fc1(x)
        x = self.relu(x)
        x = self.fc2(x)
        x = self.relu(x)
        x = self.fc3(x)
        x = self.sigmoid(x)
        return x


data1, data2, data3, data4, data5, labels = readData('data/IMU_DB3.csv')

max_accuracy = 0

X = [data1, data2, data3, data4, data5][2]
y = labels

# 数据集分割
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=0)

# 转换数据为Tensor
X_train = torch.tensor(X_train, dtype=torch.float32).reshape(-1, X.shape[1])
y_train = torch.tensor(y_train, dtype=torch.float32).view(-1, 1)
X_test = torch.tensor(X_test, dtype=torch.float32)
y_test = torch.tensor(y_test, dtype=torch.float32).view(-1, 1)

# 将numpy数组或torch.Tensor转换为TensorDataset
train_dataset = TensorDataset(X_train, y_train)
test_dataset = TensorDataset(X_test, y_test)

# 创建DataLoader
batch_size = 32

train_dataloader = DataLoader(train_dataset, batch_size=batch_size, shuffle=True)
test_dataloader = DataLoader(test_dataset, batch_size=batch_size, shuffle=False)

# 实例化模型
model = BinaryClassificationModel(X.shape[1])

# 损失函数和优化器
criterion = nn.BCELoss()  # 二分类交叉熵损失函数
optimizer = optim.Adam(model.parameters(), lr=0.01)
# 训练模型
num_epochs = 100

for epoch in range(num_epochs):
    model.train()  # 确保模型处于训练模式
    for inputs, targets in train_dataloader:
        # 前向传播
        outputs = model(inputs)
        # 计算损失
        loss = criterion(outputs, targets)
        # 反向传播和优化
        optimizer.zero_grad()  # 清零梯度
        loss.backward()  # 反向传播
        optimizer.step()  # 更新权重

    model.eval()
    with torch.no_grad():
        for inputs, targets in test_dataloader:
            outputs = model(X_test)
            predicted = (outputs >= 0.5).float()  # 将输出结果转换为0或1
            accuracy = accuracy_score(y_test, predicted)
            if accuracy > max_accuracy:
                max_accuracy = accuracy
                print(f'Epoch [{epoch + 1}/{num_epochs}], Accuracy: {accuracy:.4f}')
                # 将模型保存为onnx的格式
                torch.onnx.export(model, X_test, 'model.onnx')
            # torch.save(model.state_dict(), 'model.pth')

data = np.array([-5.2,5.6,6,-0.1,0,0,-5.3,5.6,6.1,-0.1,0,0]).astype(np.float32)
data = torch.tensor(data)
data = model(data)
predicted = (data >= 0.5).float()  # 将输出结果转换为0或1
print(predicted)