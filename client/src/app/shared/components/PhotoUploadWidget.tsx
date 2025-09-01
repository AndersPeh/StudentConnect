import { CloudUpload } from "@mui/icons-material";
import { Box, Button, Grid2, Typography } from "@mui/material";
import { useCallback, useEffect, useRef, useState } from "react";
import { useDropzone } from "react-dropzone";
import Cropper, { type ReactCropperElement } from "react-cropper";
import "cropperjs/dist/cropper.css";

type Props = {
  uploadPhoto: (file: Blob) => void;
  loading: boolean;
};

export default function PhotoUploadWidget({ uploadPhoto, loading }: Props) {
  // For storing the uploaded files. setFiles updates the files with preview URL after running inside OnDrop.
  const [files, setFiles] = useState<object & { preview: string }[]>([]);

  // useRef holds a value across re-renders and doesnt cause a re-render when it changes.
  const cropperRef = useRef<ReactCropperElement>(null);

  // The return statement of useEffect is for cleaning up purpose.
  // Whenever files change (user drops an image), trigger revokeObjectURL for cleaning up.
  useEffect(() => {
    // When React starts collecting garbage, revoke image file URLs so there will be no active pointer pointing at image file data.
    // React will garbage collect those image file data.
    return () => {
      files.forEach((file) => URL.revokeObjectURL(file.preview));
    };
    // Cleanup function is set up when files is [] as PhotoUploadWidget mounts.
    // It carries out the first cleanup when new array containing new image is added to files, but does nothing as files is [].
    // Then it sets up new cleanup function for the new files containing imageA.
    // When another new image is dropped, React will clean up the array containing imageA by revoking the URL.
    // so memory of the browser will be released.
    // When PhotoUploadWidget unmounts, the final cleanup will be carried out to revoke last remaining URL.
  }, [files]);

  // useCallback creates the onDrop function and returns the exact same function across re-renders unless its dependencies change.
  // so useDropzone receives the same onDrop function across re-renders.
  const onDrop = useCallback((acceptedFiles: File[]) => {
    // When user uploads an image, it is a raw image data and it has to be URL src to be displayed.
    setFiles(
      // Take every file, create a temporary URL that points directly to the file's data in the browser's memory.
      // then assign the URL to a new property called preview, so the temporary URL can be used as src to display the image.
      acceptedFiles.map((file) =>
        Object.assign(file, {
          // Error message shows createObjectURL(obj: Blob), so put file as Blob.
          // Because the browser keeps the URL in memory in the global context, when React runs its garbage collector, it sees URL is active in the global context,
          // so the image file data won't be garbage collected as active URL is pointing to it. Consequently, image file data will never be disposed after
          // each cancellation or successful upload. The browser will consume more and more memory to store image file data, leading to memory leaks.
          preview: URL.createObjectURL(file as Blob),
        })
      )
    );
    // Empty dependency array means create onDrop only once when the component first mount, never again as it doesnt depend on anything.
  }, []);

  // React remembers the onCrop function, it only creates a new onCrop when [uploadPhoto] prop changes.
  // As uploadPhoto is a mutation from React Query that does the same thing, onCrop will be created only once and remembered.
  const onCrop = useCallback(() => {
    // cropperRef.current? because cropperRef is null by default, it accesses the <Cropper> component.
    // React places <Cropper> instance to cropperRef due to ref={cropperRef} in Cropper.
    // .cropper includes methods of react-cropper.
    // cropperRef.current is directly referring to live component instance of Cropper.
    const cropper = cropperRef.current?.cropper;

    // .getCroppedCanvas() is retrieved from .cropper, it returns HTML <canvas> element that contains the cropped image data.
    // .toBlob((blob) converts the cropped image data to Blob (Binary Large Object), a file like object, which is needed to HTTP file upload.
    cropper?.getCroppedCanvas().toBlob(
      // .toBlob is an async function, after converting the cropped image data into Blob, it takes a callback function that uploads a photo (the newly created Blob object).
      (blob) => {
        // This is passed from ProfilePhotos.tsx to make POST request to upload photo.
        uploadPhoto(blob as Blob);
      }
    );
  }, [uploadPhoto]);

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
        {/* As user can only drop 1 image, get the first element of the files array to display. 
        Only shows the cropper the the first element of the file exists (after user uploads a photo),
        Cropper shows a crop sqaure on image uploaded src={files[0]?.preview}.
        */}
        {files[0]?.preview && (
          <Cropper
            src={files[0]?.preview}
            // To have a ref to the Cropper instance for converting the cropped image data to Blob for uploading in the next step.
            ref={cropperRef}
            style={{ height: 300, width: "90%" }}
            // Square crop
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
            <Button
              sx={{ marginY: 1, width: 300 }}
              onClick={onCrop}
              variant="contained"
              color="secondary"
              disabled={loading}
            >
              Upload
            </Button>
          </>
        )}
      </Grid2>
    </Grid2>
  );
}
