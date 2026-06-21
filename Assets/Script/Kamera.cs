using UnityEngine;

public class Kamera : MonoBehaviour
{
    [Header("Target Mobil")]
    public Transform targetMobil; // Tarik objek Mobil kamu ke sini nanti di Inspector

    [Header("Pengaturan Posisi Kamera")]
    public float jarakBelakang = 6.0f; // Seberapa jauh kamera di belakang mobil
    public float tinggiKamera = 3.0f;  // Seberapa tinggi kamera dari tanah

    [Header("Kecepatan Smooth")]
    public float smoothPosisi = 5.0f;  // Kecepatan kamera mengikuti posisi mobil
    public float smoothRotasi = 5.0f;  // Kecepatan kamera berputar mengikuti mobil

    void Start()
    {
        // Di Android, kita tidak perlu mengunci kursor mouse lagi
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Menggunakan LateUpdate agar kamera bergerak setelah mobil selesai bergerak (tidak patah-patah)
    void LateUpdate()
    {
        // Jika belum memasukkan target mobil di Inspector, jangan jalankan kode di bawah
        if (targetMobil == null) return;

        // 1. Menghitung posisi ideal kamera di belakang mobil
        Vector3 posisiTargetKamera = targetMobil.position - (targetMobil.forward * jarakBelakang) + (Vector3.up * tinggiKamera);

        // 2. Menggeser posisi kamera secara halus (Smooth Lerp) dari posisi sekarang ke posisi ideal
        transform.position = Vector3.Lerp(transform.position, posisiTargetKamera, smoothPosisi * Time.deltaTime);

        // 3. Menghitung rotasi agar kamera selalu menghadap ke arah depan mobil secara halus
        Quaternion rotasiTargetKamera = Quaternion.LookRotation(targetMobil.position - transform.position + (targetMobil.forward * 2f));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotasiTargetKamera, smoothRotasi * Time.deltaTime);
    }
}