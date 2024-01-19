import { Injectable } from "@angular/core";
import { DateHelper } from "../../helpers/date.helper";
import { Utility } from "../../utility/utility";

@Injectable({
  providedIn: 'root'
})
export class TinyEditorService {
  private baseConfig(callback?: Function) {
    console.customize(`begin fetch editor at:`, DateHelper.getTimeOnly(new Date()));
    return {
      // menubar: false,
      // plugins: [
      //   'advlist autolink lists link image charmap print preview anchor',
      //   'searchreplace visualblocks code fullscreen',
      //   'insertdatetime media table paste code help wordcount'
      // ],

      // plugins: 'autoresize'
      content_style: 'body { font-family:Helvetica,Arial,sans-serif; font-size:14px }',
      plugins: 'print preview paste importcss searchreplace autolink save directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount imagetools textpattern noneditable help charmap emoticons',
      imagetools_cors_hosts: ['picsum.photos'],
      menubar: 'file edit view insert format tools table help',
      toolbar: 'undo redo | bold italic underline strikethrough | fontselect fontsizeselect formatselect | alignleft aligncenter alignright alignjustify | outdent indent |  numlist bullist | forecolor backcolor removeformat | pagebreak | charmap emoticons | fullscreen  preview save print | insertfile image media template link anchor codesample | ltr rtl',
      toolbar_sticky: true,
      autosave_ask_before_unload: true,
      autosave_interval: '10s',
      autosave_prefix: '{path}{query}-{id}-',
      autosave_restore_when_empty: false,
      autosave_retention: '2m',
      image_advtab: true,
      relative_urls: false,
      paste_data_images: true,
      setup: (ed) => {
        ed.on('init', function (args) {
          console.customize(`finish fetch editor at:`, DateHelper.getTimeOnly(new Date()));
          if (callback) {
            callback(ed);
          }
        })
      }
      // inline: true,
      // valid_elements: 'strong,em,span[style],a[href]',
      // valid_styles: {
      //   '*': 'font-size,font-family,color,text-decoration,text-align'
      // },
      // powerpaste_word_import: 'clean',
      // powerpaste_html_import: 'clean'

    };
  }

  public getConfig(height?: number, callback?: Function) {
    const config = this.baseConfig(callback);
    config['height'] = height ? height : window.innerHeight * 0.8;
    return config;
  }

  public autoResizeConfig(callback?: Function) {
    const config = this.baseConfig(callback);
    config.plugins = 'autoresize ' + config.plugins;
    return config;
  }
}
