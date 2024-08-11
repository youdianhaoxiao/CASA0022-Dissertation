import joblib
from skl2onnx.common.data_types import *
import onnxmltools
 
# Update the input name and path for your sklearn model
input_skl_model = './xgb_classifier.pickle'
 
# input data type for your sklearn model
# 输入是Laplace算子，可以通过运行test_onnx.py报错来判断参数类型
input_data_type = ['float_input', FloatTensorType([1,2])] 
 

# Change this path to the output name and path for the ONNX model
output_onnx_model = 'model.onnx'
 
# Load your sklearn model
skl_model = joblib.load(input_skl_model)
 
# Convert the sklearn model into ONNX
onnx_model = onnxmltools.convert_sklearn(skl_model, initial_types=input_data_type)
 
# Save as protobuf
onnxmltools.utils.save_model(onnx_model, output_onnx_model)
