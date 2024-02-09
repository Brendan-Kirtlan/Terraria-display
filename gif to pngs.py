from PIL import Image
def split_gif_to_pngs(gif_path, output_folder):
    with Image.open(gif_path) as gif:
        os.makedirs(output_folder, exist_ok=True)

        for frame_index in range(gif.n_frames):
            gif.seek(frame_index)
            frame = gif.convert("RGBA")

            new_size = (frame.width // 2, frame.height // 2)
            resized_frame = frame.resize(new_size)
            
            png_filename = f"{output_folder}/frame_{frame_index:03d}.png"
            resized_frame.save(png_filename, "PNG")

            print(f"Frame {frame_index} saved as {png_filename} (Resized to {new_size})")


if __name__ == "__main__":
    import os

    #Set the path to your GIF file
    gif_path = r""
    
    #Set the output folder for PNG frames
    output_folder = r""

    split_gif_to_pngs(gif_path, output_folder)
