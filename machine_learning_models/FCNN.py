
import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import DataLoader, TensorDataset
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler
from sklearn.metrics import accuracy_score
import pandas as pd
import numpy as np
import warnings

warnings.filterwarnings("ignore")


def readData(path):
    data = pd.read_csv(path)
    data['labels'].fillna('Y', inplace=True)
    data['labels'].replace({'N': 0, 'Y': 1}, inplace=True)
    return data


class BinaryClassificationModel(nn.Module):
    def __init__(self, input_size):
        super(BinaryClassificationModel, self).__init__()
        self.layer1 = nn.Linear(input_size, 8)
        self.relu = nn.ReLU()
        self.dropout = nn.Dropout(0.5)
        self.layer2 = nn.Linear(8, 1)
        self.sigmoid = nn.Sigmoid()

    def forward(self, x):
        x = self.layer1(x)
        x = self.relu(x)
        x = self.dropout(x)
        x = self.layer2(x)
        x = self.sigmoid(x)
        return x

def export_model(model, input_size, model_name='model.onnx'):
    dummy_input = torch.randn(1, input_size, dtype=torch.float32)
    torch.onnx.export(model, dummy_input, model_name, export_params=True, opset_version=10)


data = readData('imu_s_n.csv')

# feature selection
feature_sets = {
    'set1': ['AX1', 'AY1', 'AZ1', 'GX1', 'GY1', 'GZ1'],
    'set2': ['AX1', 'AY1', 'AZ1', 'GX1', 'GY1', 'GZ1', 'AX2', 'AY2', 'AZ2', 'GX2', 'GY2', 'GZ2'],
    'set3': ['AX3', 'AY3', 'AZ3', 'GX3', 'GY3', 'GZ3', 'AX2', 'AY2', 'AZ2', 'GX2', 'GY2', 'GZ2'],
    'set4': ['AX3', 'AY3', 'AZ3', 'GX3', 'GY3', 'GZ3', 'AX1', 'AY1', 'AZ1', 'GX1', 'GY1', 'GZ1'],
    'set5': ['AX1', 'AX2', 'AX3', 'AY1', 'AY2', 'AY3', 'AZ1', 'AZ2', 'AZ3', 'GX1', 'GX2', 'GX3', 'GY1', 'GY2', 'GY3', 'GZ1', 'GZ2', 'GZ3']
}

results = {}
scaler = StandardScaler()
best_accuracy = 0
best_model = None
best_model_name = ''



# train and evaluate every feature sets
for set_name, features in feature_sets.items():
    X = data[features].values
    y = data['labels'].values

    X = scaler.fit_transform(X)
    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

    X_train = torch.tensor(X_train, dtype=torch.float32)
    y_train = torch.tensor(y_train, dtype=torch.float32).view(-1, 1)
    X_test = torch.tensor(X_test, dtype=torch.float32)
    y_test = torch.tensor(y_test, dtype=torch.float32).view(-1, 1)

    train_dataset = TensorDataset(X_train, y_train)
    test_dataset = TensorDataset(X_test, y_test)
    train_loader = DataLoader(dataset=train_dataset, batch_size=32, shuffle=True)
    test_loader = DataLoader(dataset=test_dataset, batch_size=32, shuffle=False)

    model = BinaryClassificationModel(X_train.shape[1])
    criterion = nn.BCELoss()
    optimizer = optim.Adam(model.parameters(), lr=0.001)

    for epoch in range(50):
        for inputs, labels in train_loader:
            optimizer.zero_grad()
            outputs = model(inputs)
            loss = criterion(outputs, labels)
            loss.backward()
            optimizer.step()

    with torch.no_grad():
        correct = 0
        total = 0
        for inputs, labels in test_loader:
            outputs = model(inputs)
            predicted = (outputs >= 0.5).float()
            total += labels.size(0)
            correct += (predicted == labels).sum().item()

    accuracy = correct / total
    results[set_name] = accuracy
    print(f'Feature set: {set_name}, Accuracy: {accuracy:.4f}')


# export in onnx
    if set_name == 'set1':
        export_model(model, X_train.shape[1], model_name='model1_s.onnx')
