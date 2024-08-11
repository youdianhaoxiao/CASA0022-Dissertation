import pickle
import numpy as np


# 加载之前保存的模型
with open('xgb_classifier.pickle', 'rb') as f:
    clf_loaded = pickle.load(f)

data = [1,2,3,4,5,6,7,8,9,10,11,12]
res = np.array(data).reshape(-1, 12)

result = clf_loaded.predict(res)
print(result)


# import onnxruntime as rt

# sess = rt.InferenceSession("xgb_model.onnx")
# input_name = sess.get_inputs()[0].name
# label_name = sess.get_outputs()[0].name

# pred_onx = sess.run([label_name], {input_name: res})[0]
# print(pred_onx)