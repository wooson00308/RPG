using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class CreateOverrideClips : EditorWindow
{
    [MenuItem("Tools/Create Override Clips")]
    public static void ShowWindow()
    {
        GetWindow<CreateOverrideClips>("Create Override Clips");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Create Clips"))
        {
            CreateClips();
        }
    }

    private void CreateClips()
    {
        // 1. 선택된 Animator Override Controller 가져오기
        AnimatorOverrideController overrideController = Selection.activeObject as AnimatorOverrideController;

        if (overrideController == null)
        {
            Debug.LogError("Animator Override Controller를 선택해주세요.");
            return;
        }

        // 2. 원본 Animator Controller 접근
        RuntimeAnimatorController originalController = overrideController.runtimeAnimatorController;

        if (originalController == null)
        {
            Debug.LogError("원본 Animator Controller가 없습니다.");
            return;
        }

        // 3. 애니메이션 클립 목록 가져오기
        AnimationClip[] originalClips = originalController.animationClips;

        //저장할 폴더 경로
        string folderPath = GetSavePath(overrideController);

        // 4. 클립 생성 및 저장, 할당
        // 리스트 생성
        List<KeyValuePair<AnimationClip, AnimationClip>> clipOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        // 기존에 override되어 있는 clip들 먼저 추가.
        overrideController.GetOverrides(clipOverrides);

        Dictionary<AnimationClip, AnimationClip> clipOverrideMap = new Dictionary<AnimationClip, AnimationClip>();

        // 맵에 추가
        foreach (var clipOverride in clipOverrides)
        {
            if (clipOverride.Value != null)
            {
                clipOverrideMap.Add(clipOverride.Key, clipOverride.Value);
            }

        }


        foreach (AnimationClip originalClip in originalClips)
        {
            // 이미 오버라이드 된 클립이 있다면 생성 건너뜀
            if (clipOverrideMap.ContainsKey(originalClip))
            {
                Debug.Log($"{originalClip.name}은 이미 오버라이드 되어있어 생성을 건너뜁니다");
                continue;
            }

            // '@' 뒤의 문자열 추출
            string clipNameAfterAt = originalClip.name.Contains("@") ? originalClip.name.Split('@')[1] : originalClip.name;


            // 4-1. 새 파일 이름 만들기
            string newClipName = $"{overrideController.name}@{clipNameAfterAt}.anim";

            string newClipPath = Path.Combine(folderPath, newClipName); // 폴더 경로와 조합

            // 4-2. AnimationClip 객체 생성
            AnimationClip newClip = new AnimationClip();
            newClip.name = newClipName; // 새 클립 이름 설정 (파일 이름과는 별개)

            // ********** Loop Time 설정 **********
            // AnimationClipSettings 가져오기
            AnimationClipSettings originalSettings = AnimationUtility.GetAnimationClipSettings(originalClip);
            AnimationClipSettings newSettings = AnimationUtility.GetAnimationClipSettings(newClip);

            // loopTime 설정 복사
            newSettings.loopTime = originalSettings.loopTime;

            //새 클립에 settings 적용
            AnimationUtility.SetAnimationClipSettings(newClip, newSettings);



            // 4-3. 에셋 생성
            AssetDatabase.CreateAsset(newClip, newClipPath);
            Debug.Log($"{newClipPath}에 클립 생성 완료");


            //새로운 override 추가
            clipOverrideMap.Add(originalClip, newClip);


        }

        // 5. override 적용
        // 리스트 clear
        clipOverrides.Clear();

        // Dictionary 순회
        foreach (var clipOverride in clipOverrideMap)
        {
            clipOverrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(clipOverride.Key, clipOverride.Value));
        }
        overrideController.ApplyOverrides(clipOverrides);


        AssetDatabase.Refresh(); // 변경 사항 갱신
        Debug.Log("클립 생성 및 Override 적용 완료!");
    }


    private string GetSavePath(AnimatorOverrideController aoc)
    {
        // AnimatorOverrideController 에셋의 경로를 가져옵니다.
        string assetPath = AssetDatabase.GetAssetPath(aoc);

        // 경로에서 디렉토리 부분만 추출합니다.
        string directoryPath = Path.GetDirectoryName(assetPath);

        return directoryPath;
    }
}