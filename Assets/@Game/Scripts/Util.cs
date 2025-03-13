using UnityEngine;

public static class Util
{
    const double EPSILON = 0.0001;

    /// <summary>
    /// LookAt 2D���� �ٶ󺸴� ���� ��ȯ
    /// </summary>
    public static Vector3 LookAt2D(Transform transform, Transform targetPos)
    {
        Vector3 dir = targetPos.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 lookDir = new Vector3(0, 0, angle);
        return lookDir;
    }

    public static Vector3 LookAt2D(Vector2 transform, Vector2 targetPos)
    {
        Vector3 dir = targetPos - transform;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 lookDir = new Vector3(0, 0, angle);
        return lookDir;
    }

    public static bool IsEqual(double x, double y) // �� �Լ�.
    {
        return (((x - EPSILON) < y) && (y < (x + EPSILON)));
    }
}
