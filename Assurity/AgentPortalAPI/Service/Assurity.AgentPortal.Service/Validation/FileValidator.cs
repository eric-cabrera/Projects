namespace Assurity.AgentPortal.Service.Validation
{
    using System.Net;
    using Assurity.AgentPortal.Contracts.Send;
    using Assurity.AgentPortal.Contracts.Send.Enums;
    using Assurity.AgentPortal.Utilities.Configs;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class FileValidator : IFileValidator
    {
        private static readonly List<string> PermittedExtensions = new()
        {
            ".doc",
            ".docx",
            ".jpeg",
            ".jpg",
            ".pdf",
            ".png",
            ".txt",
            ".tif",
            ".tiff"
        };

        /// <summary>
        /// Contains a list of file extensions and their "file signatures", which is a set of data
        /// (represented here in hex) that can be used to identify or verify the content of a file.
        /// <see cref="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
        /// <see cref="https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/mvc/models/file-uploads/samples/3.x/SampleApp/Utilities/FileHelpers.cs"/>
        /// These are also known as magic numbers or magic bytes.
        /// </summary>
        private static readonly Dictionary<string, List<byte[]>> FileSignature = new()
        {
            // Compound File Binary Format, a container format defined by Microsoft COM. It can be
            // the equivalent of files and directories. It is used by Windows Installer and for
            // documents in older versions of Microsoft Office. It can be used by other program as
            // well that rely on the COM and OLE APIs.
            // This follows the Wikipedia article.
            // doc, xls, ppt, msi, msg
            {
                ".doc",
                new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }
                }
            },

            // The docx file signature is the same as a whole host of other formats.
            // This follows the Wikipedia article.
            // zip file format and formats based on it, such as EPUB, JAR, ODF, OOXML
            // zip, aar, apk, doc, epub, ipa, jar, kmz, maff, msix, odp, ods, odt, pk3, pk4, pptx, usdz, vsdx, xlsx, xpi
            {
                ".docx",
                new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 }
                }
            },

            // The exe format is specified here in order to detect if a .txt is actually an executable,
            // due to the inability to detect .txt files via a file signature.
            {
                ".exe",
                new List<byte[]>
                {
                    new byte[] { 0x00, 0x00, 0x03, 0xF3 }, // Amiga Hunk executable file
                    new byte[] { 0x4D, 0x5A }, // exe, dll, mui, sys, scr, cpl, ocx, ax, iec, ime, rs, tsp, fon, efi
                    new byte[] { 0x5A, 0x4D }, // exe (DOS ZM executable and its descendants (rare))
                    new byte[] { 0x7F, 0x45, 0x4C, 0x46 }, // Executable and Linkable Format
                    new byte[] { 0x64, 0x65, 0x78, 0x0A, 0x30, 0x33, 0x35, 0x00 }, // Dalvik Executable
                    new byte[] { 0x53, 0x5A, 0x44, 0x44, 0x88, 0xF0, 0x27, 0x33 }, // Microsoft compressed file in Quantum format
                    new byte[] { 0x4A, 0x6F, 0x79, 0x21 } // Preferred Executable Format
                }
            },

            // This follows the example provided by Microsoft, not Wikipedia (since that differs).
            {
                ".jpeg",
                new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
                }
            },

            // This follows the example provided by Microsoft, not Wikipedia (since that differs).
            {
                ".jpg",
                new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
                }
            },

            // Portable Document Format (PDF) file format developed by Adobe.
            // This follows the Wikipedia article.
            {
                ".pdf",
                new List<byte[]>
                {
                    new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }
                }
            },

            // Image encoded in the Portable Network Graphics (PNG) format.
            // This follows the Wikipedia and Microsoft articles.
            {
                ".png",
                new List<byte[]>
                {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                }
            },

            // Tagged Image File Format
            // This follows the Wikipedia article.
            {
                ".tif",
                new List<byte[]>
                {
                    new byte[] { 0x49, 0x49, 0x2A, 0x00 },
                    new byte[] { 0x4D, 0x4D, 0x00, 0x2A }
                }
            },

            // Tagged Image File Format
            // This follows the Wikipedia article.
            {
                ".tiff",
                new List<byte[]>
                {
                    new byte[] { 0x49, 0x49, 0x2A, 0x00 },
                    new byte[] { 0x4D, 0x4D, 0x00, 0x2A }
                }
            },

            // Text files
            {
                ".txt",
                new List<byte[]>
                {
                    new byte[] { 0xEF, 0xBB, 0xBF }, // UTF-8 byte order mark, commonly seen in text files
                    new byte[] { 0xFF, 0xFE }, // UTF-16LE byte order mark, commonly seen in text files
                    new byte[] { 0xFE, 0xFF }, // UTF-16BE byte order mark, commonly seen in text files
                    new byte[] { 0xFF, 0xFE, 0x00, 0x00 }, // UTF-32LE byte order mark for text
                    new byte[] { 0x00, 0x00, 0xFE, 0xFF }, // UTF-32BE byte order mark for text
                    new byte[] { 0x0E, 0xFE, 0xFF } // SCSU byte order mark for text
                }
            }
        };

        public FileValidator(
            IConfigurationManager configurationManager,
            ILogger<FileValidator> logger)
        {
            ConfigurationManager = configurationManager;
            Logger = logger;
        }

        private IConfigurationManager ConfigurationManager { get; }

        private ILogger<FileValidator> Logger { get; }

        public bool IsMultipartContentType(string? contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                && contentType.Contains("multipart/", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<File> ProcessFormFile(
            IFormFile formFile,
            ModelStateDictionary modelState)
        {
            var file = new File
            {
                Name = WebUtility.HtmlEncode(formFile.FileName)
            };

            // Check the file length. This check doesn't catch files that only have
            // a BOM (Byte Order Mark) as their content.
            if (formFile.Length == 0)
            {
                modelState.AddModelError(
                    formFile.Name,
                    $"{file.Name} file is empty.");

                return file;
            }

            var maximumFileLengthInBytes = ConfigurationManager.TakeActionMaximumFileLengthInBytes;

            if (formFile.Length > maximumFileLengthInBytes)
            {
                var megabyteSizeLimit = maximumFileLengthInBytes / 1048576;

                modelState.AddModelError(
                    formFile.Name,
                    $"{file.Name} file exceeds {megabyteSizeLimit:N1} MB.");

                return file;
            }

            try
            {
                using var memoryStream = new MemoryStream();

                await formFile.CopyToAsync(memoryStream);

                // Check the content length in case the file's only content was a BOM and
                // the content is actually empty after removing the BOM.
                if (memoryStream.Length == 0)
                {
                    modelState.AddModelError(
                        formFile.Name,
                        $"{file.Name} file is empty.");
                }

                if (!IsValidFileExtensionAndSignature(formFile.FileName, memoryStream))
                {
                    var errorMessage = $"{file.Name} file type isn't permitted or " +
                        "the file's signature doesn't match the file's extension.";
                    modelState.AddModelError(formFile.Name, errorMessage);
                }
                else
                {
                    file.FileType = GetFileType(formFile.FileName);
                    file.Bytes = memoryStream.ToArray();

                    return file;
                }
            }
            catch (Exception exception)
            {
                var exceptionMessage = $"{file.Name} file upload failed.";

                modelState.AddModelError(formFile.Name, exceptionMessage);

                Logger.LogError(exception, exceptionMessage);
            }

            return file;
        }

        private static FileType GetFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".doc" => FileType.Doc,
                ".docx" => FileType.Docx,
                ".jpeg" => FileType.Jpeg,
                ".jpg" => FileType.Jpg,
                ".pdf" => FileType.Pdf,
                ".png" => FileType.Png,
                ".txt" => FileType.Txt,
                ".tif" => FileType.Tif,
                ".tiff" => FileType.Tiff,
                _ => throw new ArgumentOutOfRangeException(nameof(extension), "The provided extension is not supported."),
            };
        }

        /// <summary>
        /// Validates whether the file extension is one of the accepted file formats
        /// and whether the uploaded file data matches the file extension.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsValidFileExtensionAndSignature(
            string fileName, Stream data)
        {
            if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
            {
                return false;
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !PermittedExtensions.Contains(extension))
            {
                return false;
            }

            // Currently unable to detect a txt file based on a signature, since the first bytes
            // are just the first few characters of text in the file, which could be anything.
            // Therefore, at least run a check to make sure it isn't a disguised executable.
            if (extension == ".txt")
            {
                // Returns false if the data is found to be an executable.
                return !IsValidFileSignature(data, ".exe");
            }

            return IsValidFileSignature(data, extension);
        }

        private static bool IsValidFileSignature(Stream data, string extension)
        {
            data.Position = 0;

            using var binaryReader = new BinaryReader(data);
            var signatures = FileSignature[extension];
            var headerBytes = binaryReader.ReadBytes(signatures.Max(bytes => bytes.Length));

            return signatures
                .Any(signature => headerBytes.Take(signature.Length)
                .SequenceEqual(signature));
        }
    }
}