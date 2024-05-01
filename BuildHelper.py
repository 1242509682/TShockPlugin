import os
import glob
import shutil
import sys
import zipfile
import urllib.request
def zip_files_in_folder(folder_path, zip_file_path):
    # Create a ZipFile object in write mode
    with zipfile.ZipFile(zip_file_path, 'w') as zipf:
        # Iterate over each file in the folder
        for foldername, subfolders, filenames in os.walk(folder_path):
            for filename in filenames:
                # Create the complete filepath by concatenating the folder and filename
                file_path = os.path.join(foldername, filename)
                # Add file to the zip file
                # The arcname parameter is used to store the file without any folder structure
                zipf.write(file_path, arcname=os.path.basename(file_path))
    print(f"生成压缩包: {zip_file_path}")

def md_to_pdf(file_name):
    #print(f"pandoc --pdf-engine=xelatex  -V mainfont='Noto Serif CJK SC' -V geometry:margin=0.5in  {file_name} -o {file_name.replace('.md', '.pdf')}")
    os.system(f"pandoc --pdf-engine=xelatex  -V mainfont=LXGWWenKaiMono-Regular.ttf -V geometry:margin=0.5in --template eisvogel.tex  {file_name} -o {file_name.replace('.md', '.pdf')}")
    
if __name__ == '__main__':
    print(f"😋😋😋打包脚本By Cai...")
    build_type = sys.argv[1]
    print(f"😋开始删除json文件")
    for file in glob.glob(os.path.join(f"out/{build_type}/", "*.json")):
        os.remove(file)
        print(f"删除文件: {file}")
    print("删除json文件成功~")

    # Get the current working directory
    print("😋开始移动README.md")
    cwd = os.getcwd()
    #shutil.copyfile("README.md",f"out/{build_type}/README.md")
    # Iterate over all directories in the current working directory
    for dir_name in os.listdir(cwd):
        dir_path = os.path.join(cwd, dir_name)
        # Check if it is a directory
        if os.path.isdir(dir_path):
            # Iterate over all files in the directory
            for file_name in os.listdir(dir_path):
                # Check if the file is a .csproj file
                try:
                    if file_name.endswith('.csproj'):
                        # Construct the source path of the README.md file
                        source_path = os.path.join(dir_path, 'README.md')
                        # Construct the destination path in the out/{build_type} directory with the same name as the .csproj file
                        destination_path = os.path.join(cwd, 'out', f'{build_type}', file_name.replace('.csproj', '.md'))
                        # Copy the README.md file to the destination path
                        shutil.copyfile(source_path, destination_path)
                        print(f"找到README.md({destination_path})")
                except:
                    print(f"README移动失败({file_name})")
    print("移动README.md成功~")
    if build_type == "Release":
        print("😋准备转换PDF")
        urllib.request.urlretrieve("https://raw.githubusercontent.com/lxgw/LxgwWenKai/main/fonts/TTF/LXGWWenKaiMono-Regular.ttf", "LXGWWenKaiMono-Regular.ttf")
        urllib.request.urlretrieve("https://raw.githubusercontent.com/Wandmalfarbe/pandoc-latex-template/master/eisvogel.tex", "eisvogel.tex")
        # 指定的目录
        directory = '/usr/share/texmf/fonts/opentype/public/lm/'

        # 指定的文件
        specified_file = 'LXGWWenKaiMono-Regular.ttf'

        # 遍历指定目录
        for filename in os.listdir(directory):
            if os.path.isfile(os.path.join(directory, filename)):
                # 复制指定文件到目标文件，覆盖目标文件
                shutil.copy2(specified_file, os.path.join(directory, filename))

        for file_name in os.listdir(f"out/{build_type}"):
            if file_name.endswith('.md'):
                md_to_pdf(f"{cwd}/out/{build_type}/{file_name}")
                os.remove(f"{cwd}/out/{build_type}/{file_name}")
                print(f"{file_name}转换成功...")
        
        print("PDF转换完成～")
        # 调用函数来压缩文件夹中的所有文件
        # 注意：这里需要替换为实际的文件夹路径和zip文件路径
    print("😋准备打包插件")
    zip_files_in_folder("out", "Plugins.zip")
    print("😋😋😋插件打包成功~")





