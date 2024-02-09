from PIL import Image

def png_to_2d_array(file_path):
    img = Image.open(file_path)
    width, height = img.size
    pixel_data = list(img.getdata())
    rgba_array = [pixel_data[i:i + width] for i in range(0, len(pixel_data), width)]
    return rgba_array

def create_reformatted_chum(output_folder, width, height, rgba_values, index):
    
    try:
        import os
        os.makedirs(output_folder, exist_ok=True)
        output_file_path = os.path.join(output_folder, "reformatted" + str(index) + ".chum")

        with open(output_file_path, 'wb') as file:
            file.write(width.to_bytes(2,'big'))
            file.write(height.to_bytes(2,'big'))

            for i in range(len(rgba_values)):
                for j in range(len(rgba_values[i])):
                    file.write(rgba_values[i][j][0].to_bytes(1,'big'))
                    file.write(rgba_values[i][j][1].to_bytes(1,'big'))
                    file.write(rgba_values[i][j][2].to_bytes(1,'big'))
            

        print(f"Reformatted file '{output_file_path}' created successfully.")

    except Exception as e:
        print(f"Error creating reformatted file: {e}")




