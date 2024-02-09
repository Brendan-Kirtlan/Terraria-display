from PIL import Image
import os

def convert_webp_to_png(input_folder, output_folder):
    if not os.path.exists(output_folder):
        os.makedirs(output_folder)
    files = os.listdir(input_folder)

    for file_name in files:
        if file_name.lower().endswith(".webp"):
            input_path = os.path.join(input_folder, file_name)
            output_path = os.path.join(output_folder, os.path.splitext(file_name)[0] + ".png")

            with Image.open(input_path) as img:
                img.save(output_path, "PNG")

            print(f"Converted: {file_name}")

if __name__ == "__main__":
    input_folder = r""
    output_folder = r""

    convert_webp_to_png(input_folder, output_folder)
