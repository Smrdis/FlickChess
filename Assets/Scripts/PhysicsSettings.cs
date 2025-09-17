using UnityEngine;

[CreateAssetMenu(fileName = "PhysicsSettings", menuName = "Game/Physics Settings")]
public class PhysicsSettings : ScriptableObject
{
    [Header("Unit Physics")]
    [Range(0.1f, 2f)]
    public float unitMass = 1f;
    
    [Range(0f, 2f)]
    public float unitDrag = 0.5f;
    
    [Range(0f, 2f)]
    public float unitAngularDrag = 0.5f;
    
    [Header("Ground Detection")]
    [Range(0.1f, 3f)]
    public float groundCheckDistance = 1.5f;
    
    [Range(0f, 1f)]
    public float slidingDrag = 1f;
    
    [Range(0f, 1f)]
    public float fallingDrag = 0.1f;
    
    [Header("Bounce Settings")]
    [Range(0f, 10f)]
    public float bounceForce = 5f;
    
    [Range(0f, 5f)]
    public float minBounceVelocity = 2f;
    
    [Range(0f, 1f)]
    public float bounceRandomness = 0.3f;
    
    [Header("Constraints")]
    public bool freezeRotationOnGround = true;
    public bool allowRotationInAir = true;
}