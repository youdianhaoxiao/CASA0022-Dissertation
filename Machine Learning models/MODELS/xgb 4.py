from sklearn.preprocessing import StandardScaler
from xgboost import XGBClassifier
import xgboost as xgb
from readData import *
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import accuracy_score, classification_report
import pickle
# import skl2onnx
# from skl2onnx.common.data_types import FloatTensorType,Int64TensorType
# from skl2onnx import convert_sklearn, update_registered_converter
# from skl2onnx.common.shape_calculator import (
#     calculate_linear_classifier_output_shapes,
# )  # noqa
# import onnxmltools
# from onnxmltools.convert.xgboost.operator_converters.XGBoost import (
#     convert_xgboost,
# )  # noqa
# import onnxmltools.convert.common.data_types



data1, data2, data3, data4, data5, labels = readData('./imu_DB.csv')

# 使用随机森林进行训练  X为data1 y为labels 其中80%作为训练集 20%作为测试集
datas = [data1, data2, data3, data4, data5]
learning_rate = [0.01, 0.015, 0.025, 0.05, 0.1]
max_depth = [3, 5, 6, 7, 9, 12, 15, 17, 25]
min_child_weight = [1, 3, 5, 7]
gamma = [0, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1, 0.3, 0.5, 0.7, 0.9, 1]
subsample = [0.6, 0.7, 0.8, 0.9, 1]
colsample_bytree = [0.6, 0.7, 0.8, 0.9, 1]
max_accuracy = 0

learning_rate, max_depth, min_child_weight, gamma, subsample, colsample_bytree = 0.05, 7, 3, 0.3, 0.9, 0.9
X = data3
y = labels
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=0)

xgb_model = XGBClassifier(max_depth=max_depth,
                          learning_rate=learning_rate,
                          n_estimators=100,  # 使用多少个弱分类器
                          objective='binary:logistic',
                          booster='gbtree',
                          gamma=gamma,
                          min_child_weight=min_child_weight,
                          max_delta_step=0,
                          subsample=subsample,
                          colsample_bytree=colsample_bytree,
                          reg_alpha=0,
                          reg_lambda=1,
                          seed=0)
xgb_model.fit(X_train, y_train)
# 保存模型
with open('xgb_classifier.pickle', 'wb') as f:
    pickle.dump(xgb_model, f)

# # Convert into ONNX format
# # initial_type = [('float_input', FloatTensorType([None, X_train.shape[1]]))]
# initial_type = [('float_input', FloatTensorType([None, 12]))]

# # Define the final types for label and probabilities
# final_type = [
#     ('label', Int64TensorType([None, 1])),       # Class label: typically an integer
#     ('probabilities', FloatTensorType([None, 2])) # Probabilities: typically a float, and 2 for binary classification
# ]
# update_registered_converter(
#     XGBClassifier,
#     "XGBoostXGBClassifier",
#     calculate_linear_classifier_output_shapes,
#     convert_xgboost,
#     options={"nocl": [True, False], "zipmap": [False]},
# )
# onx =convert_sklearn(xgb_model, initial_types=initial_type, final_types=final_type,options={"zipmap": False})

# # Save to an ONNX file
# with open("xgb_model.onnx", "wb") as f:
#     f.write(onx.SerializeToString())
