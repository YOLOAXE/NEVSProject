using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.MultipleAdditiveScenes;

public class TPMouseLookPlayerFinder : MonoBehaviour
{
    [SerializeField] float minDistance = 10F;
    [SerializeField] float maxDistance = 40F;
    [SerializeField] float sensitivity = 5F;
    [SerializeField] float minimumX = -360F;
    [SerializeField] float maximumX = 360F;
    [SerializeField] float minimumY = -60F;
    [SerializeField] float maximumY = 60F;
    [SerializeField] float scrollSpeed = 20F;
    [SerializeField] float maxOffsetCam = 1f;
    [SerializeField] LayerMask layerMaskPlayer = 1;
    [SerializeField] private GameObject mainCamera = null;
    [SerializeField] private GameObject targetPlayer = null;
    [SerializeField] private GameObject[] allPlayer = null;
    [SerializeField] private GameObject owner = null;
    [SerializeField] private int indice = 0;
    float posCamZ = -4F;
    [SerializeField] private float rotationX = 0F;
    [SerializeField] private float rotationY = 0F;
    Quaternion originalRotation;
    RaycastHit HitDistance, HitOffset;
    Vector3 MoveCamRayCast = new Vector3(0f, 0f, 0f); 

    void Start()
    {
        mainCamera.transform.LookAt(transform);
        transform.parent = null;
        owner = GameObject.FindWithTag("Player");
    }

    void OnEnable()
    {
        if (owner)
        {
            transform.position = owner.transform.position + new Vector3(0, 0.5f, 0);
        }
    }

    void Update()
    {
        VueThirdPerson();
        if(targetPlayer)
        {
            transform.position = targetPlayer.transform.position + new Vector3(0, 0.5f, 0);
        }
        if(Input.GetButtonDown("Fire1"))
        {
            findPlayer();
        }
    }

    private void findPlayer()
    {
        allPlayer = GameObject.FindGameObjectsWithTag("nlPlayer");
        if(++indice >= allPlayer.Length)
        {
            indice = 0;
        }
        if (allPlayer.Length > 0)
        {
            targetPlayer = allPlayer[indice];
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) { angle += 360F; }
        if (angle > 360F) { angle -= 360F; }
        return Mathf.Clamp(angle, min, max);
    }

    void VueThirdPerson()
    {
        rotationX += Input.GetAxis("Mouse X");
        rotationY += Input.GetAxis("Mouse Y");
        rotationX = ClampAngle(rotationX, minimumX, maximumX);
        rotationY = ClampAngle(rotationY, minimumY, maximumY);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
        transform.rotation =  xQuaternion * yQuaternion;
        posCamZ += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        if (posCamZ > -minDistance) { posCamZ = -minDistance; }
        if (posCamZ < -maxDistance) { posCamZ = -maxDistance; }
        mainCamera.transform.localPosition = new Vector3(0f, 0f, posCamZ);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out HitDistance, -posCamZ, layerMaskPlayer))
        {
            if (HitDistance.distance > minDistance)
            {
                mainCamera.transform.position = HitDistance.point + new Vector3(0f, 0.5f, 0f);
            }
            else
            {
                mainCamera.transform.localPosition = new Vector3(0f, 1f, 0f);
            }
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * HitDistance.distance, Color.red);
        }

    }
}