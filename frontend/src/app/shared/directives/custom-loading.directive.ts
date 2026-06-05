import {
  ApplicationRef,
  ComponentRef,
  createComponent,
  Directive,
  EmbeddedViewRef,
  EnvironmentInjector,
  inject,
  Input,
  Renderer2,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { ProgressSpinner } from 'primeng/progressspinner';

@Directive({
  selector: '[appCustomLoading]',
})
export class CustomLoadingDirective {
  private readonly templateRef = inject(TemplateRef);
  private readonly vcr = inject(ViewContainerRef);
  private readonly renderer = inject(Renderer2);
  private readonly appRef = inject(ApplicationRef);
  private readonly envInjector = inject(EnvironmentInjector);

  private viewRef: EmbeddedViewRef<any> | null = null;
  private overlayEl: HTMLElement | null = null;
  private spinnerRef: ComponentRef<ProgressSpinner> | null = null;

  @Input({ alias: 'appCustomLoading' }) set loading(value: boolean) {
    if (!this.viewRef) {
      this.viewRef = this.vcr.createEmbeddedView(this.templateRef);
    }

    const rootEl = this.viewRef.rootNodes[0] as HTMLElement;

    if (value) {
      if (this.overlayEl) return;

      this.renderer.setStyle(rootEl, 'position', 'relative');

      this.overlayEl = this.renderer.createElement('div');
      this.renderer.setStyle(this.overlayEl, 'position', 'absolute');
      this.renderer.setStyle(this.overlayEl, 'inset', '0');
      this.renderer.setStyle(this.overlayEl, 'background', 'rgba(200, 200, 200, 0.4)');
      this.renderer.setStyle(this.overlayEl, 'display', 'flex');
      this.renderer.setStyle(this.overlayEl, 'align-items', 'center');
      this.renderer.setStyle(this.overlayEl, 'justify-content', 'center');
      this.renderer.setStyle(this.overlayEl, 'border-radius', 'inherit');
      this.renderer.setStyle(this.overlayEl, 'z-index', '9999');

      this.spinnerRef = createComponent(ProgressSpinner, {
        environmentInjector: this.envInjector,
      });
      this.appRef.attachView(this.spinnerRef.hostView);

      this.renderer.appendChild(this.overlayEl, this.spinnerRef.location.nativeElement);
      this.renderer.appendChild(rootEl, this.overlayEl);
    } else {
      if (!this.overlayEl) return;

      this.renderer.removeChild(rootEl, this.overlayEl);
      this.overlayEl = null;

      this.appRef.detachView(this.spinnerRef!.hostView);
      this.spinnerRef!.destroy();
      this.spinnerRef = null;
    }
  }
}
