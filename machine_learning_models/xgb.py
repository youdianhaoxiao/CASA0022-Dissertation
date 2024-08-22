from sklearn.preprocessing import StandardScaler
from xgboost import XGBClassifier
from readData import *
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import accuracy_score, classification_report

data1, data2, data3,data4,data5, labels = readData('./imu_s_n.csv')


datas =  [data1, data2, data3,data4,data5]

learning_rate = [0.01, 0.015, 0.025, 0.05, 0.1]
max_depth = [3, 5, 6, 7, 9, 12, 15, 17, 25]
min_child_weight = [1, 3, 5, 7]
gamma = [0, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1, 0.3, 0.5, 0.7, 0.9, 1]
subsample = [0.6, 0.7, 0.8, 0.9, 1]
colsample_bytree = [0.6, 0.7, 0.8, 0.9, 1]
max_accuracy = 0

for num in range(len(datas)):
    X = datas[num]
    y = labels
    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=0)
    for i in learning_rate:
        for j in max_depth:
            for k in min_child_weight:
                for l in gamma:
                    for m in subsample:
                        for n in colsample_bytree:
                            xgb_model = XGBClassifier(max_depth=j,
                                                      learning_rate=i,
                                                      n_estimators=100, 
                                                      objective='binary:logistic',
                                                      booster='gbtree',
                                                      gamma=l,
                                                      min_child_weight=k,
                                                      max_delta_step=0,
                                                      subsample=m,
                                                      colsample_bytree=n,
                                                      reg_alpha=0,
                                                      reg_lambda=1,
                                                      seed=0)
                            xgb_model.fit(X_train, y_train)
                            y_pred = xgb_model.predict(X_test)
                            accuracy = accuracy_score(y_test, y_pred)
                            if accuracy > max_accuracy:
                                max_accuracy = accuracy
                                print('Datas',f'data{num}','accuracy:', accuracy)
