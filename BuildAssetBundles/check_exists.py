import os.path
import csv
from global_variables import *

with open(startPath+'.csv', 'rUb') as csvfile:
    reader = csv.DictReader(csvfile)
    ctr = 1
    for row in reader:
        modelFolderPath = os.path.join(startPath,row['fullId'][4:])
        modelFolderPath = os.path.join(modelFolderPath, 'model')
        ctr = ctr+1
        if(os.path.isfile(modelFolderPath)):
            #print("Exists! " +str(ctr) + "/8509: "+ modelFolderPath)
            
            if (ctr%100 ==0):
                print(str(ctr) + "/" + str(total_count))
        else:
            print("*********ERROR NOT FOUND **********")