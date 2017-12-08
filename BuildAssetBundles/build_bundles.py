import os.path
import csv
import shutil
import os
import subprocess
from global_variables import *


with open(startPath+'.csv', 'rUb') as csvfile:
    reader = csv.DictReader(csvfile)
    ctr = 1
    ctrfail = 0
    for row in reader:
        modelFolderPath = os.path.join(startPath,row['fullId'][4:])
        modelFilePath = os.path.join(modelFolderPath, 'modelios')
        print(str(ctr) + "/"+str(total_count)+": "+ modelFolderPath)
        ctr = ctr + 1
        if(os.path.isfile(modelFilePath)):
            print("Exists! " +str(ctr) + "/"+str(total_count)+": "+ modelFilePath)
            continue
        shutil.rmtree(workingDir, ignore_errors=True)
        shutil.copytree(blankDir, workingDir)
        shutil.copytree(modelFolderPath, os.path.join(workingDir, r'Assets/model'))
        print("Copied OBJ, running Unity")
        #command = '\"C:/Program\\ Files/Unity/Editor/Unity.exe\" -batchmode -quit -projectPath \""+workingDir+"\" -executeMethod CreateAssetBundles.BuildAllAssetBundles'
        try:
            subprocess.check_call(["C:/Program Files/Unity/Editor/Unity.exe", "-batchmode", "-quit", "-projPath", "\""+workingDir+"\"", "-executeMethod", "CreateAssetBundles.BuildAllAssetBundles"] )
        except subprocess.CalledProcessError as e:
            output = e.output
            returncode = e.returncode
            ctrfail = ctrfail + 1
            print(str(ctrfail) + ": Subprocess failed! Error: " + output)
        print("Built AssetBundle, copying back")
        modelAssetBundlePath = os.path.join(workingDir, modelAssetBundle)
        modelManifestAssetBundlePath = os.path.join(workingDir, modelManifestAssetBundle)
        while not(os.path.isfile(modelAssetBundlePath)):
            print("file not yet created!" + modelAssetBundlePath)
        shutil.copy(modelAssetBundlePath, os.path.join(modelFolderPath, 'modelios'))
        shutil.copy(modelManifestAssetBundlePath, os.path.join(modelFolderPath, 'modelios.manifest'))
        print("Done, removing temp files")
        shutil.rmtree(workingDir)
            
#Make sure model folder has been copied into the blank template, then run this
#"C:\Program Files\Unity5.3.5f1\Editor\Unity.exe" -batchmode -quit -projectPath "C:/Users/aditya/Documents/Unity/BuildAssetBundles" -executeMethod CreateAssetBundles.BuildAllAssetBundles