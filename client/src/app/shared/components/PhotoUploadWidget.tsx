import { CloudUpload } from "@mui/icons-material";
import { Box, Grid2, Typography } from "@mui/material";
import { useCallback, useRef, useState } from "react";
import { useDropzone } from "react-dropzone";
import Cropper, { type ReactCropperElement } from "react-cropper";
import "cropperjs/dist/cropper.css";

export default function PhotoUploadWidget() {
  // For storing the uploaded files.
  const [files, setFiles] = useState<object & { preview: string }[]>([]);

  const cropperRef = useRef<ReactCropperElement>(null);

  // useCallback creates the onDrop function and returns the exact same function across re-renders unless its dependencies change.
  // so useDropzone receives the same onDrop function across re-renders.
  const onDrop = useCallback((acceptedFiles: File[]) => {
    setFiles(
      acceptedFiles.map((file) =>
        Object.assign(file, {
          // Error message shows createObjectURL(obj: Blob), so put file as Blob.
          preview: URL.createObjectURL(file as Blob),
        })
      )
    );
    // Empty dependency array means create onDrop only once when the component first mount, never again as it doesnt depend on anything.
  }, []);

  // Default properties from React Dropzone. useDropzone takes the same onDrop function across re-renders,
  // so it doesnt have to rerun internal logic every re-render.
  const { getRootProps, getInputProps, isDragActive } = useDropzone({ onDrop });

  return (
    <Grid2 container spacing={3}>
      <Grid2 size={4}>
        <Typography variant="overline" color="secondary">
          Step 1 - Add photo
        </Typography>
        {/* getRootProps makes Box look and act like a dropzone. It triggers the hidden input getInputProps. */}
        <Box
          {...getRootProps()}
          sx={{
            border: "dashed 3px",
            borderColor: isDragActive ? "green" : "#eee",
            borderRadius: "5px",
            paddingTop: "30px",
            textAlign: "center",
            height: "280px",
          }}
        >
          {/* getInputProps opens the file selection window of the operating system. */}
          <input {...getInputProps()} />
          <CloudUpload sx={{ fontSize: 80 }} />
          <Typography variant="h5">Drop image here</Typography>
        </Box>
      </Grid2>
      <Grid2 size={4}>
        <Typography variant="overline" color="secondary">
          Step 2 - Resize image
        </Typography>
        {/* As user can only drop 1 image, get the first element of the files array to display. */}
        {files[0]?.preview && (
          <Cropper
            src={files[0]?.preview}
            style={{ height: 300, width: "90%" }}
            // Square images
            initialAspectRatio={1}
            aspectRatio={1}
            preview=".img-preview"
            guides={false}
            viewMode={1}
            background={false}
          />
        )}
      </Grid2>
      <Grid2 size={4}>
        {files[0]?.preview && (
          <>
            <Typography variant="overline" color="secondary">
              Step 3 - Preview and Upload
            </Typography>
            <div
              className="img-preview"
              style={{ width: 300, height: 300, overflow: "hidden" }}
            />
          </>
        )}
      </Grid2>
    </Grid2>
  );
}
