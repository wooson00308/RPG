using UnityEngine;
using UnityEditor;
using System.IO;

public class RenameFilesInFolder : EditorWindow
{
    [MenuItem("Tools/Rename Files In Folder")]
    public static void ShowWindow()
    {
        GetWindow<RenameFilesInFolder>("Rename Files");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Rename Files"))
        {
            RenameFiles();
        }
    }

    private void RenameFiles()
    {
        // 선택된 폴더 가져오기 (Project 뷰에서 선택)
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

        foreach (Object selectedAsset in selectedAssets)
        {
            string path = AssetDatabase.GetAssetPath(selectedAsset);

            // 폴더가 아니면 건너뛰기
            if (!AssetDatabase.IsValidFolder(path))
            {
                continue;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            string folderName = dirInfo.Name; // 폴더 이름 가져오기

            FileInfo[] fileInfos = dirInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly); // 폴더 내의 모든 파일 가져오기 (하위 폴더 제외)

            foreach (FileInfo fileInfo in fileInfos)
            {
                // .meta 파일은 건너뜁니다.
                if (fileInfo.Extension == ".meta")
                {
                    continue;
                }

                string oldFileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                string newFileName;

                // "@" 문자를 기준으로 파일 이름 처리
                if (oldFileName.Contains("@"))
                {
                    string[] parts = oldFileName.Split('@');
                    // "@" 앞부분이 어떤 문자열이든 폴더 이름으로 교체
                    newFileName = folderName + "@" + parts[1];
                }
                else
                {
                    // "@" 문자가 없으면, 전체 파일 이름을 폴더 이름으로 교체
                    newFileName = folderName;
                }


                string relativeFilePath = Path.Combine(path, fileInfo.Name); // 기존 파일의 상대 경로
                string newRelativeFilePath = Path.Combine(path, newFileName + fileInfo.Extension); // 새 파일의 상대 경로


                // 파일 이름 변경 실행
                AssetDatabase.RenameAsset(relativeFilePath, newFileName + fileInfo.Extension);
            }
            AssetDatabase.Refresh(); // 변경 사항 갱신
        }
        Debug.Log("파일 이름 변경 완료!");
    }
}