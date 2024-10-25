using System.IO.Compression;
using iTextSharp.text;
using iTextSharp.text.pdf;

class CBZtoPDFConverter
{
    public static void ConvertCbzToPdf(string cbzPath, string pdfPath)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        // Extract images from CBZ
        ZipFile.ExtractToDirectory(cbzPath, tempDir);

        // Get all image files
        string[] imageFiles = Directory.GetFiles(tempDir, "*.*", SearchOption.AllDirectories);

        Array.Sort(imageFiles);

        // Create PDF document
        using (FileStream fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
        using (Document doc = new Document(PageSize.LETTER))
        using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
        {
            doc.Open();

            foreach (string imageFile in imageFiles)
            {
                if (imageFile.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    imageFile.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    imageFile.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imageFile);
                    img.ScaleToFit(PageSize.LETTER.Width, PageSize.LETTER.Height);
                    img.Alignment = Element.ALIGN_CENTER;
                    doc.Add(img);
                    doc.NewPage();
                }
            }

            doc.Close();
        }

        // Clean up temporary files
        Directory.Delete(tempDir, true);
    }

    static void Main(string[] args)
    {
        // Specify the folder containing CBZ files
        string cbzFolderPath = "cbzfiles"; // Replace with your folder path
        string pdfFolderPath = "pdffiles";

        // Get all CBZ files in the folder
        string[] cbzFiles = Directory.GetFiles(cbzFolderPath, "*.cbz", SearchOption.TopDirectoryOnly);

        foreach (string cbzFile in cbzFiles)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(cbzFile);
            string outputPdfPath = Path.Combine(pdfFolderPath, fileNameWithoutExtension + ".pdf");

            try
            {
                ConvertCbzToPdf(cbzFile, outputPdfPath);
                Console.WriteLine($"Successfully converted: {cbzFile} to {outputPdfPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to convert: {cbzFile}. Error: {ex.Message}");
            }
        }

        Console.WriteLine("All conversions complete!");
    }
}
