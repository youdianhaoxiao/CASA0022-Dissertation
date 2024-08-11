import pandas as pd
import warnings

# 忽略所有警告
warnings.filterwarnings("ignore")

"""
Reads the data from the csv file
"""


def readData(path):
    data = pd.read_csv(path)
    # 将labels这一列的nan替换为”Y“
    data['labels'].fillna('Y', inplace=True)
    # 替换所有的N为0
    data['labels'].replace('N', 0, inplace=True)
    data['labels'].replace('Y', 1, inplace=True)
    data1 = data[['AX1', 'AY1', 'AZ1', 'GX1', 'GY1', 'GZ1']].values
    data2 = data[['AX1', 'AY1', 'AZ1', 'GX1', 'GY1', 'GZ1', 'AX2', 'AY2', 'AZ2', 'GX2', 'GY2', 'GZ2']].values
    data3 = data[['AX3', 'AY3', 'AZ3', 'GX3', 'GY3', 'GZ3', 'AX2', 'AY2', 'AZ2', 'GX2', 'GY2', 'GZ2']].values
    data4 = data[['AX3', 'AY3', 'AZ3', 'GX3', 'GY3', 'GZ3', 'AX1', 'AY1', 'AZ1', 'GX1', 'GY1', 'GZ1']].values
    data5 = data[['AX1', 'AX2', 'AX3', 'AY1', 'AY2', 'AY3', 'AZ1', 'AZ2', 'AZ3', 'GX1', 'GX2', 'GX3', 'GY1', 'GY2', 'GY3', 'GZ1','GZ2', 'GZ3']].values
    labels = data['labels']
    return data1, data2, data3, data4,data5,labels


readData("./imu_DB3.csv")
